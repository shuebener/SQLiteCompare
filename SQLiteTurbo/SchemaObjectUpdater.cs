using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Data;
using System.Data.SQLite;
using SQLiteParser;
using log4net;

namespace SQLiteTurbo
{
    /// <summary>
    /// This class is responsible to perform updates to the left,right or both database
    /// objects that are specified in the schema comparison item. The update is done
    /// based on the schema strings provided for the left object and the right object.
    /// </summary>
    public class SchemaObjectUpdater : AbstractWorker
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaObjectUpdater"/> class.
        /// </summary>
        /// <param name="item">The item that contains the DDL objects for the left and right
        /// databases</param>
        /// <param name="leftSchema">The left schema - this is needed in case we are
        /// updating a table schema because we'll need to re-create all associated
        /// indexes and triggers</param>
        /// <param name="rightSchema">The right schema - this is needed in case we are
        /// updating a table schema because we'll need to re-create all associated
        /// indexes and triggers</param>
        /// <param name="leftdb">The left database file path</param>
        /// <param name="rightdb">The right database file path</param>
        /// <param name="leftSQL">The SQL string for creating the left database object (after the user
        /// edited it).</param>
        /// <param name="rightSQL">The SQL string for creating the right database object (after the
        /// user edited it).</param>
        public SchemaObjectUpdater(SchemaComparisonItem item, 
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> leftSchema,
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> rightSchema,
            string leftdb, string rightdb,
            string leftSQL, string rightSQL,
            bool skipNullRows)
            : base("Schema Object Updater")
        {
            _item = item;
            _leftSchema = leftSchema;
            _rightSchema = rightSchema;
            _leftSQL = leftSQL;
            _rightSQL = rightSQL;
            _leftdb = leftdb;
            _rightdb = rightdb;
            _skipNullRows = skipNullRows;

            // Initialize the lexical scanner and the parser objects that will be used
            // to analyze all SQLite schema objects.
            _parser.scanner = _scanner;
        }
        #endregion

        #region AbstractWorker Overrided Methods
        protected override void DoWork()
        {
            if (_item.LeftDdlStatement is SQLiteCreateTableStatement)
                UpdateTableSchema();
            else if (_item.LeftDdlStatement is SQLiteCreateIndexStatement)
                UpdateIndexSchema();
            else if (_item.LeftDdlStatement is SQLiteCreateTriggerStatement)
                UpdateTriggerSchema();
            else if (_item.LeftDdlStatement is SQLiteCreateViewStatement)
                UpdateViewSchema();
            else
            {
                throw new ApplicationException("Illegal object type found in comparison item [" +
                    _item.LeftDdlStatement.GetType().FullName + "]");
            } // else

            // Update the comparison result for this item.
            if (_item.LeftDdlStatement.Equals(_item.RightDdlStatement))
                _item.Result = ComparisonResult.Same;
            else
                _item.Result = ComparisonResult.DifferentSchema;
        }

        protected override bool IsDualProgress
        {
            get
            {
                // We support dual (primary+secondary) notifications only for tables
                // because tables may require data re-insertion which can be a very
                // lengthy process.
                if (_item.LeftDdlStatement is SQLiteCreateTableStatement)
                    return true;
                else
                    return false;
            }
        }
        #endregion

        #region Private Methods
        private void UpdateViewSchema()
        {
            if (_leftSQL != null)
            {
                SQLiteDdlStatement stmt = _item.LeftDdlStatement;
                UpdateViewSchema(_leftdb, _leftSchema, _leftSQL, ref stmt);
                _item.LeftDdlStatement = stmt;
            }
            if (_rightSQL != null)
            {
                SQLiteDdlStatement stmt = _item.RightDdlStatement;
                UpdateViewSchema(_rightdb, _rightSchema, _rightSQL, ref stmt);
                _item.RightDdlStatement = stmt;
            }
        }

        private void UpdateTriggerSchema()
        {
            if (_leftSQL != null)
            {
                SQLiteDdlStatement stmt = _item.LeftDdlStatement;
                UpdateTriggerSchema(_leftdb, _leftSchema, _leftSQL, ref stmt);
                _item.LeftDdlStatement = stmt;
            }
            if (_rightSQL != null)
            {
                SQLiteDdlStatement stmt = _item.RightDdlStatement;
                UpdateTriggerSchema(_rightdb, _rightSchema, _rightSQL, ref stmt);
                _item.RightDdlStatement = stmt;
            }
        }

        private void UpdateIndexSchema()
        {
            if (_leftSQL != null)
            {
                SQLiteDdlStatement stmt = _item.LeftDdlStatement;
                UpdateIndexSchema(_leftdb, _leftSchema, _leftSQL, ref stmt, 25, 50);
                _item.LeftDdlStatement = stmt;
            }
            if (_rightSQL != null)
            {
                SQLiteDdlStatement stmt = _item.RightDdlStatement;
                UpdateIndexSchema(_rightdb, _rightSchema, _rightSQL, ref stmt, 65, 80);
                _item.RightDdlStatement = stmt;
            }
        }

        private void UpdateTableSchema()
        {
            if (_leftSQL != null)
            {
                SQLiteDdlStatement stmt = _item.LeftDdlStatement;
                UpdateTableSchema(_leftdb, _leftSchema, _leftSQL, ref stmt);
                _item.LeftDdlStatement = stmt;
            }
            if (_rightSQL != null)
            {
                SQLiteDdlStatement stmt = _item.RightDdlStatement;
                UpdateTableSchema(_rightdb, _rightSchema, _rightSQL, ref stmt);
                _item.RightDdlStatement = stmt;
            }
        }

        /// <summary>
        /// Update the table schema in the specified database file with the updated table schema
        /// found in the <paramref name="sql"/>sql parameter.
        /// </summary>
        /// <param name="dbpath">The path to the database where the table should be updated</param>
        /// <param name="schema">The schema dictionary of that database file. This is needed because
        /// we want to update the dictionary table statement after successfully completing the update.</param>
        /// <param name="sql">The updated SQL string needed in order to re-create the table</param>
        /// <param name="orig">The original table DDL statement object. If everything goes OK, this
        /// will be replaced by the updated DDL statement object.</param>
        private void UpdateTableSchema(string dbpath, 
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> schema, 
            string sql, ref SQLiteDdlStatement orig)
        {
            sql = Utils.StripComments(sql);
            _scanner.SetSource(sql, 0);

            // Request the parser to parse the SQL statement
            bool ok = _parser.Parse();
            if (!ok)
                throw new ApplicationException("schema has incorrect syntax");

            // Minimal sanity checking
            SQLiteDdlStatement stmt = SQLiteDdlMain.GetStatement();
            if (!(stmt is SQLiteCreateTableStatement))
                throw new ApplicationException("schema is not a CREATE TABLE statement");
            if (!stmt.ObjectName.Equals(orig.ObjectName))
                throw new ApplicationException("the table name is incorrect");

            // If the "updated" table is the same as the original table - return immediatly
            if (stmt.Equals(orig))
                return;

            // There are two options:
            //
            // 1. The user simply added new column(s) to the table and these were added AFTER
            //    the last table column. In this case we can simply do a series of ALTER TABLE
            //    commands in order to bring the table to the desired SQL schema. This is the fastest
            //    most surest route to make the table update.
            //
            // 2. Anything else that changed will require us to completely re-create the table
            //    from scratch. This can be a very lengthy process (especially for large tables),
            //    but it can be unavoidable because in SQLite we cannot use the ALTER TABLE
            //    command for anything other than adding new columns.
            //    As an example:
            //
            //    2.1. Changes in column constraint(s) will require a complete re-write of the table.
            //    2.2. When a column is removed in the updated table schema.
            //    2.3. When an existing table constraint is added, changed or removed - a complete
            //         re-write of the table will be required.

            // Open connection to the database, drop the existing table object and
            // re-create it using the updated sql schema
            using (SQLiteConnection conn = Utils.MakeDbConnection(dbpath, true))
            {
                conn.Open();

                SQLiteTransaction tx = conn.BeginTransaction();
                try
                {
                    // If running simple ALTER TABLE commands can do the job, the 'added' list
                    // will contain the list of columns that should be added to the existing table
                    // in order to perform the update.
                    List<SQLiteColumnStatement> added = null;

                    // Test if running simple ALTER TABLE ADD COLUMN commands is enough to perform
                    // the table update in the database.
                    if (Utils.AlterTableIsPossible(orig, stmt, ref added))
                    {
                        // Simple ALTER TABLE commands can do the job
                        AlterTableByAddingNewColumns((SQLiteCreateTableStatement)stmt, added, conn, tx);
                    }
                    else
                    {
                        // Unfortunately we'll have to re-create the table from scratch.
                        AlterTableByReCreatingFromScratch(schema,
                            (SQLiteCreateTableStatement)orig, (SQLiteCreateTableStatement)stmt, conn, ref tx);
                    } // else

                    tx.Commit();
                }
                catch (Exception se)
                {
                    tx.Rollback();
                    throw;
                } // catch
            } // using

            // Update the comparison item so that the change is reflected in the
            // schema comparison view grid.
            orig = stmt;

            // Update the schema dictionary with the updated version
            schema[SchemaObject.Table][stmt.ObjectName.ToString().ToLower()] = stmt;            
        }

        /// <summary>
        /// This method is used to re-create the entire table from scratch in cases when a simple
        /// ALTER TABLE cannot do the job. It creates a temporary table with the schema of the updated
        /// table, moves all the data rows from the original table to the temporary table, drops the original
        /// table and renames the temporary table to have the same name of the original table and adds all
        /// the triggers and indexes that were dropped when the original table was removed.
        /// </summary>
        /// <param name="orig">The original table DDL statement</param>
        /// <param name="updated">The updated DDL statement for creating the table</param>
        /// <param name="conn">The SQLite connection</param>
        /// <param name="tx">The containing transaction</param>
        private void AlterTableByReCreatingFromScratch(
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> schema, 
            SQLiteCreateTableStatement orig, SQLiteCreateTableStatement updated, 
            SQLiteConnection conn, ref SQLiteTransaction tx)
        {
            NotifyPrimaryProgress(false, 10, "Recreating the table from scratch..", null);

            // Check how many data rows exist in the original table. This information is used in two ways:
            // 1. To provide progress notifications during the copy operation.
            // 2. If the original table is NON-EMPTY then we can't copy the original table rows
            //    to the updated table if the updated table contains columns that are NOT NULL without
            //    a suitable DEFAULT clause (which has to be a non-null default). 
            //    In such a case we'll issue an error before doing anything harmful..

            // This list will be used in the ValidateTableCopy method to store a list of columns
            // that turned from being NULLable in the original table schema to being NOT NULL in the updated table 
            // schema. These columns may cause the copy operation to fail so they require special attention.
            List<SQLiteColumnStatement> checkColumns = null;

            // Check if the original table is empty or not.
            SQLiteCommand cntcmd = new SQLiteCommand(@"SELECT COUNT(*) FROM " + orig.ObjectName.ToString(), conn, tx);
            long count = (long)cntcmd.ExecuteScalar();
            if (count > 0)
            {
                NotifyPrimaryProgress(false, 15, "Validating table copy operation..", null);

                // When the original table contains any rows we risk that the copy operation will fail
                // due to NULL/NOT NULL problems. Since we don't want the copy operation to fail midways - we
                // check for the possible error conditions right from the start. 
                // The ValidateTableCopy method performs this check.
                string errmsg = null;
                bool canRestart;
                bool allowIfRowsAreSkipped = false;
                bool ok = ValidateTableCopy(orig, updated, conn, tx, ref allowIfRowsAreSkipped, ref errmsg, ref checkColumns, out canRestart);
                if (!ok)
                {
                    // There are two cases when the validation can fail:
                    // 1. The updated table schema contains a NOT NULL column without a suitable DEFAULT constraint
                    //    and the original table schema does not contain that column at all. When this occurs - the
                    //    copy operation cannot proceed because we don't have non NULL data that can be inserted to
                    //    new column(s).
                    // 2. The updated table schema contains a NOT NULL column and the original table schema contains
                    //    the same column as NULLable. In this case the copy may be performed but only if there are no
                    //    rows that actually have NULL values in that column or if the user decided to allow us to
                    //    skip such rows altogether.
                    if (!allowIfRowsAreSkipped || !_skipNullRows)
                        throw new UpdateTableException(canRestart, errmsg);
                } // if
            }

            CheckCancelled();

            NotifyPrimaryProgress(false, 20, "Creating temporary table..", null);

            // First step is to create a table with the exact schema as the updated table
            // but give it a temporary name until all the data rows are copied from the original
            // table. This is done so that a possible software/system crash will not cause the
            // data in the original table to be lost.
            string tmpname = Utils.GetTempName(updated.ObjectName.ToString());
            SQLiteCommand create = new SQLiteCommand(updated.ToStatement(tmpname), conn, tx);
            create.ExecuteNonQuery();

            tx.Commit();
            tx = conn.BeginTransaction();

            try
            {
                // Next step is to copy all the data from the original table to the temporary table
                // Note: Do this step only if there are actually data rows in the original table.
                if (count > 0)
                {
                    NotifyPrimaryProgress(false, 25, "Copying rows from original table to the updated table..", null);

                    // Prepare the array of column names that turned NOT NULL in the target but were NULLable
                    // in the original table schema. This is done for performance reasons.
                    string[] checkcols = null;
                    if (checkColumns != null)
                    {
                        checkcols = new string[checkColumns.Count];
                        for (int i = 0; i < checkColumns.Count; i++)
                            checkcols[i] = SQLiteParser.Utils.Chop(checkColumns[i].ObjectName.ToString());
                    }

                    // Prepare the array that contains all column names for columns that belong to both
                    // the original table and to the updated table. Only these columns will be copied
                    // from the original table to the updated table.
                    string[] inscols;
                    string[] choppedInsCols;
                    string[] prmnames;
                    PrepareInsertColumns(orig, updated, out inscols, out choppedInsCols, out prmnames);

                    long offset = 0;
                    SQLiteCommand insert = PrepareInsertCommand(tmpname, updated, inscols, prmnames, conn, tx);
                    SQLiteCommand select = new SQLiteCommand("SELECT * FROM " + orig.ObjectName.ToString(), conn, tx);
                    using (SQLiteDataReader reader = select.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            offset++;

                            // Skip rows that have at least one field whose value is NULL and whose
                            // column is defined as NOT NULL in the updated table schema.
                            if (checkcols != null && SkipRow(reader, checkcols))
                                continue;

                            // Fill the INSERT parameters and execute the INSERT command
                            for (int i = 0; i < inscols.Length; i++)
                                Utils.AssignParameterValue(insert.Parameters[prmnames[i]], reader[choppedInsCols[i]]);
                            insert.ExecuteNonQuery();

                            // Commit every 1000 insertions
                            if (offset % 1000 == 0)
                            {
                                CheckCancelled();

                                tx.Commit();
                                tx = conn.BeginTransaction();

                                int progress = (int)(100.0 * offset / count);
                                NotifySecondaryProgress(false, progress, "" + offset + " rows were copied so far", null);
                            } // if
                        } // while
                    } // using

                    // Commit any remaining inserts
                    if (offset % 1000 != 0)
                    {
                        CheckCancelled();

                        tx.Commit();
                        tx = conn.BeginTransaction();
                    }

                    NotifySecondaryProgress(true, 100, "Finished copying table rows to the updated table", null);
                } // if

                NotifyPrimaryProgress(false, 50, "Dropping the original table and renaming the temporary table", null);

                // Next step is to drop the original table and alter the name of the temporary table
                // to have the same name as the original table.
                SQLiteCommand drop = new SQLiteCommand("DROP TABLE " + orig.ObjectName.ToString(), conn, tx);
                drop.ExecuteNonQuery();
                SQLiteCommand rename = new SQLiteCommand("ALTER TABLE " + tmpname + " RENAME TO " + orig.ObjectName.ToString(), conn, tx);
                rename.ExecuteNonQuery();

                CheckCancelled();

                NotifyPrimaryProgress(false, 70, "Creating table indexes ..", null);

                // Next step is to re-create the indexes and trigger objects. This has to be done because
                // when a table is dropped so are all of its indexes and triggers.

                // First add all indexes
                AddTableIndexes(orig.ObjectName, schema, conn, tx);

                NotifyPrimaryProgress(false, 90, "Creating table triggers ..", null);

                // Next add all triggers
                AddObjectTriggers(orig.ObjectName, schema, conn, tx);
            }
            catch (Exception ex)
            {
                tx.Rollback();
                tx = conn.BeginTransaction();

                // In case something wrong happened - we'll drop the temporary table before
                // re-throwing the exception
                SQLiteCommand droptemp = new SQLiteCommand("DROP TABLE " + tmpname, conn, tx);
                droptemp.ExecuteNonQuery();

                tx.Commit();
                tx = conn.BeginTransaction();

                throw;
            } // catch
        }

        /// <summary>
        /// Prepares arrays of column names that belong to both the original table and to the
        /// updated table. Only these columns will be included in INSERT statements. In addition
        /// it computes the list of corresponding parameter names to use in the INSERT statement.
        /// </summary>
        /// <param name="orig">The original table schema</param>
        /// <param name="updated">The updated table schema</param>
        /// <param name="inscols">The array of column names to be included in the INSERT statement.</param>
        /// <param name="prmnames">The array of parameter names to be included in the INSERT statement</param>
        private void PrepareInsertColumns(SQLiteCreateTableStatement orig, SQLiteCreateTableStatement updated, 
            out string[] inscols, out string[] chopped, out string[] prmnames)
        {
            List<string> ccols = new List<string>();
            List<string> icols = new List<string>();
            List<string> pnames = new List<string>();

            foreach (SQLiteColumnStatement ocol in orig.Columns)
            {
                foreach (SQLiteColumnStatement ucol in updated.Columns)
                {
                    if (ocol.ObjectName.Equals(ucol.ObjectName))
                    {
                        icols.Add(ocol.ObjectName.ToString());
                        ccols.Add(SQLiteParser.Utils.Chop(ocol.ObjectName.ToString()));
                        pnames.Add(Utils.GetColumnParameterName(ocol.ObjectName.ToString()));
                        break;
                    }
                } // foreach
            } // foreach

            inscols = icols.ToArray();
            chopped = ccols.ToArray();
            prmnames = pnames.ToArray();
        }

        /// <summary>
        /// Decides if the current row contains NULL values in columns that became NOT NULL
        /// in the updated table schema. If this is the case - the row will be skipped.
        /// </summary>
        /// <param name="reader">The reader object (positioned on the current row)</param>
        /// <param name="checkColumns">The list of columns that became NOT NULL in the updated
        /// table schema</param>
        /// <returns>TRUE if the row should be skipped, FALSE if the row should be copied</returns>
        private bool SkipRow(SQLiteDataReader reader, string[] colnames)
        {
            for (int i = 0; i < colnames.Length; i++)
            {
                if (reader[colnames[i]] == DBNull.Value)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Creates a SQL INSERT command that can be used to insert rows to the specified
        /// table.
        /// </summary>
        /// <param name="table">The name of the table</param>
        /// <param name="conn">The SQLite connection</param>
        /// <param name="tx">The containing transaction</param>
        /// <returns>A command object that can be used to insert rows to the table</returns>
        private SQLiteCommand PrepareInsertCommand(string table, 
            SQLiteCreateTableStatement stmt,
            string[] inscols, string[] prmnames, SQLiteConnection conn, SQLiteTransaction tx)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO " + table + "(");
            for (int i = 0; i < inscols.Length; i++)
            {
                sb.Append(inscols[i]);
                if (i < inscols.Length - 1)
                    sb.Append(", ");
            } // for
            sb.Append(") VALUES (");
            for (int i = 0; i < prmnames.Length; i++)
            {
                sb.Append(prmnames[i]);
                if (i < prmnames.Length - 1)
                    sb.Append(", ");
            } // for
            sb.Append(")");

            SQLiteCommand res = new SQLiteCommand(sb.ToString(), conn, tx);
            for (int i = 0; i < prmnames.Length; i++)
            {
                SQLiteColumnStatement colstmt = Utils.GetColumnByName(stmt, inscols[i]);
                DbType dbtype = Utils.GetDbType(colstmt.ColumnType);
                res.Parameters.Add(prmnames[i], dbtype);
            } // for

            return res;
        }

        /// <summary>
        /// Add all the index objects for the specified table.
        /// </summary>
        /// <param name="name">The table name</param>
        /// <param name="schema">The schema that contains all index objects for that table</param>
        /// <param name="conn">The SQLite connection</param>
        /// <param name="tx">The containing transaction</param>
        private void AddTableIndexes(SQLiteObjectName name, 
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> schema, 
            SQLiteConnection conn, SQLiteTransaction tx)
        {
            foreach (SQLiteCreateIndexStatement stmt in schema[SchemaObject.Index].Values)
            {
                if (SQLiteParser.Utils.Chop(stmt.OnTable.ToLower()) == SQLiteParser.Utils.Chop(name.ToString().ToLower()))
                {
                    SQLiteCommand create = new SQLiteCommand(stmt.ToString(), conn, tx);
                    create.ExecuteNonQuery();
                }

                CheckCancelled();
            } // foreach
        }

        /// <summary>
        /// This method checks the validity of copying all data rows from the original table into
        /// a table with the specified updated table schema. There are 8 important types of copy
        /// operations and we need to check for 2 of them in order to make sure that the copy
        /// operation will not fail.
        /// </summary>
        /// <param name="orig">The original table schema definition</param>
        /// <param name="updated">The updated table schema definition</param>
        /// <param name="conn">The SQLite connection to use</param>
        /// <param name="tx">The containing transaction</param>
        /// <param name="allowIfRowsAreSkipped">In case the validation has failed this value will indicate
        /// if table copy can nevertheless execute on the condition that rows that contain NULL values in
        /// the original table will not be copied.</param>
        /// <param name="errmsg">In case the validation fails - returns the error message</param>
        /// <param name="checkColumns">In case the operation can succeed but with skipped data rows - this
        /// list will contain the columns whose field values need to be checked if we are to copy the row
        /// to the updated table.</param>
        /// <returns>
        /// TRUE indicates that the copy operation can be performed, FALSE indicates that there
        /// is a problem to copy the original table to the table with the updated schema (but see explanation
        /// for the <paramref name="allowIfRowsAreSkipped"/> parameter).
        /// </returns>
        private bool ValidateTableCopy(SQLiteCreateTableStatement orig, SQLiteCreateTableStatement updated, 
            SQLiteConnection conn, SQLiteTransaction tx, ref bool allowIfRowsAreSkipped, ref string errmsg,
            ref List<SQLiteColumnStatement> checkColumns, out bool canRestart)
        {
            canRestart = false;

            // Following are the 8 relevant types of copying that can take place when copying data rows
            // from the original table to a table that has the updated table schema:
            //
            // Original Table Column           Updated Table Column            Result
            // -------------------------+--------------------------------+--------------------
            //       NULL               |          NOT NULL              |  DEPENDS ON DATA
            //     NOT NULL             |            NULL                |       OK
            //     NOT NULL             |          NOT NULL              |       OK
            //       NULL               |            NULL                |       OK
            //        N/A               |            NULL                |       OK
            //        N/A               |          NOT NULL              |      ERROR
            //        N/A               |       NOT NULL DEFAULT         |       OK
            //        N/A               |         NULL DEFAULT           |       OK
            // -------------------------------------------------------------------------------
            //
            // Note that the case when a column does not exist in the original table (N/A) and exists
            // in the updated table schema with NOT NULL constraint without a non-null DEFAULT is a clear 
            // error since we can't insert even a single row to a table that has such a column without 
            // specifying its data (and we don't have the data because the original table does not 
            // contain this column).
            // 
            // The other case is when the column existed in the original table but allowed NULL values and
            // the updated table schema contains a column with the same name but that does not allow NULL
            // values. In this case we'll have to search the original table for rows that have NULL values
            // in that column. If we'll find such a row we'll have to abort the copy operation because we can't
            // copy these values to the table with the updated table schema (since it does not allow NULL values
            // in that column).

            // The only columns to check are those columns that belong to both the original and the updated
            // table schemas as well as those columns that exist only in the updated table schema. Columns
            // that existed only in the original table schema but not in the updated table schema do not 
            // affect the copying process so they are irrelevent to the validity of the copying.
            checkColumns = null;
            List<SQLiteColumnStatement> check = new List<SQLiteColumnStatement>();
            for(int i=0; i<updated.Columns.Count; i++)
            {
                SQLiteColumnStatement ucol = updated.Columns[i];

                // Try to find the matching column in the original table
                SQLiteColumnStatement ocol = null;
                foreach (SQLiteColumnStatement col in orig.Columns)
                {
                    if (col.ObjectName.Equals(ucol.ObjectName))
                    {
                        ocol = col;
                        break;
                    }
                } // foreach

                // If not found - make sure that the column from the updated table schema allows NULL
                // values or has a non-null DEFAULT constraint. 
                // Otherwise - the validation fails. This is so because the original table does
                // not contain this column so we don't have any real data to place in it during the copy
                // operation.
                if (ocol == null)
                {
                    // If the column in the updated schema is NOT NULL and does not have a suitable
                    // DEFAULT (a non-null DEFAULT) then we have an error.
                    if (Utils.IsNotNullColumnWithoutDefault(ucol))
                    {
                        allowIfRowsAreSkipped = false;
                        errmsg = "Column " + ucol.ObjectName.ToString() +
                            " has a NOT NULL constraint but there is no matching column in the" +
                            " original table to copy from. Please make the column NULLable or specify\r\n" +
                            " an appropriate DEFAULT clause.";
                        return false;
                    } // if
                }
                else
                {
                    // In case the original column had a NULL constraint and the updated column has
                    // a NOT NULL constraint we'll add the column to a list of columns that will be
                    // checked later using data analysis.
                    if (ColumnSupportsNullValues(ocol) && ColumnHasNotNullConstraint(ucol))
                        check.Add(ucol);
                } // else
            } // for

            // Check if there are any columns whose data needs to be checked in order to validate
            // the copy operation.
            if (check.Count > 0)
            {
                // SELECT COUNT(*) FROM table WHERE Col1 IS NULL OR Col2 IS NULL OR Col3 IS NULL

                // In order to check this we'll build a simple EXISTS query that checks 
                // if there are any data rows where any of the relevant column values is null.
                StringBuilder cb = new StringBuilder();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < check.Count; i++)
                {
                    sb.Append(check[i].ObjectName.ToString() + " IS NULL");
                    if (i < check.Count - 1)
                        sb.Append(" OR ");

                    cb.Append(check[i].ObjectName.ToString());
                    if (i < check.Count - 1)
                        cb.Append(", ");
                } // for

                // Note: This is (potentially) a long operation because it may need to scan the entire table
                //       in order to find even a single row with NULL values in the appropriate columns.
                SQLiteCommand cnulls = new SQLiteCommand("SELECT COUNT(*) FROM " + 
                    orig.ObjectName.ToString() +
                    " WHERE " + sb.ToString(), conn, tx);
                long count = (long)cnulls.ExecuteScalar();
                if (count > 0)
                {
                    // The original table contains rows where the column values is NULL. These rows will not
                    // be able to be copied to the target table so the copy operation will fail.
                    errmsg = "The table contains "+count+" rows with NULL value for one\r\n" +
                        "or more of the columns (" + cb.ToString() + ").\r\n" +
                        "Since the updated schema specifies that these columns are NOT NULL, such rows\r\n" +
                        "cannot be moved to the updated table.\r\n" +
                        "If you really want to update the table then you can restart the operation and\r\n" +
                        "specify that these rows should be skipped.";
                    allowIfRowsAreSkipped = true;
                    checkColumns = check;
                    canRestart = true;
                    return false;
                }
            } // if

            return true;
        }

        /// <summary>
        /// Checks if the specified column has a NOT NULL constraint.
        /// </summary>
        /// <param name="col">The column to check</param>
        /// <returns>TRUE if the column has a NOT NULL constraint</returns>
        private bool ColumnHasNotNullConstraint(SQLiteColumnStatement col)
        {
            if (col.ColumnConstraints == null)
                return false;

            foreach (SQLiteColumnConstraint ccon in col.ColumnConstraints)
            {
                SQLiteNullColumnConstraint nulcon = ccon as SQLiteNullColumnConstraint;
                if (nulcon != null && !nulcon.IsNull)
                    return true;
            } // foreach

            return false;
        }

        /// <summary>
        /// Checks if the specified column supports NULLability
        /// </summary>
        /// <param name="col">The column to check</param>
        /// <returns>TRUE if the column supports NULL values</returns>
        private bool ColumnSupportsNullValues(SQLiteColumnStatement col)
        {
            if (col.ColumnConstraints == null)
                return true;

            foreach (SQLiteColumnConstraint ccon in col.ColumnConstraints)
            {
                SQLiteNullColumnConstraint nulcon = ccon as SQLiteNullColumnConstraint;
                if (nulcon != null && !nulcon.IsNull)
                    return false;
            } // foreach

            return true;
        }

        /// <summary>
        /// This method is responsible to use ALTER TABLE commands in order to add the specified
        /// list of columns definitions to the table schema
        /// </summary>
        /// <param name="stmt">The table DDL statement object</param>
        /// <param name="added">The list of added table column definitions</param>
        /// <param name="conn">The database connection</param>
        /// <param name="tx">The containing transaction</param>
        private void AlterTableByAddingNewColumns(
            SQLiteCreateTableStatement stmt,
            List<SQLiteColumnStatement> added, SQLiteConnection conn, SQLiteTransaction tx)
        {
            for (int i = 0; i < added.Count; i++)
            {
                SQLiteCommand alter = new SQLiteCommand("ALTER TABLE "+stmt.ObjectName.ToString()+
                    " ADD COLUMN " + added[i].ToString(), conn, tx);
                alter.ExecuteNonQuery();
            } // for
        }

        /// <summary>
        /// Update the index schema in the specified database file with the updated index schema
        /// found in the <paramref name="sql"/>sql parameter.
        /// </summary>
        /// <param name="dbpath">The path to the database where the index should be updated</param>
        /// <param name="schema">The schema dictionary of that database file. This is needed because
        /// we want to update the dictionary trigger statement after successfully completing the update.</param>
        /// <param name="sql">The updated SQL string needed in order to re-create the view</param>
        /// <param name="orig">The original view DDL statement object. If everything goes OK, this
        /// will be replaced by the updated DDL statement object.</param>
        /// <param name="start">Progress value used in the first progress notification</param>
        /// <param name="end">Progress value used in the second progress notification</param>
        private void UpdateIndexSchema(string dbpath, 
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> schema, 
            string sql, ref SQLiteDdlStatement orig, int start, int end)
        {
            sql = Utils.StripComments(sql);
            _scanner.SetSource(sql, 0);

            // Request the parser to parse the SQL statement
            bool ok = _parser.Parse();
            if (!ok)
                throw new ApplicationException("schema has incorrect syntax");

            // Minimal sanity checking
            SQLiteDdlStatement stmt = SQLiteDdlMain.GetStatement();
            if (!(stmt is SQLiteCreateIndexStatement))
                throw new ApplicationException("schema is not a CREATE INDEX statement");
            if (!stmt.ObjectName.Equals(orig.ObjectName))
                throw new ApplicationException("the index name is incorrect");

            // Open connection to the database, drop the existing index object and
            // re-create it using the updated sql schema
            using (SQLiteConnection conn = Utils.MakeDbConnection(dbpath, true))
            {
                conn.Open();

                SQLiteTransaction tx = conn.BeginTransaction();
                try
                {
                    NotifyPrimaryProgress(false, start, "Dropping index " + stmt.ObjectName.ToString()+
                        " (may take some time)", null);

                    // First drop the trigger
                    SQLiteCommand drop = new SQLiteCommand(
                        @"DROP INDEX " + stmt.ObjectName.ToString(), conn, tx);
                    drop.ExecuteNonQuery();

                    NotifyPrimaryProgress(false, end, "Creating index " + stmt.ObjectName.ToString() +
                        " (may take some time)", null);

                    // Then re-create it from scratch
                    SQLiteCommand create = new SQLiteCommand(stmt.ToString(), conn, tx);
                    create.ExecuteNonQuery();

                    tx.Commit();
                }
                catch (SQLiteException se)
                {
                    tx.Rollback();
                    throw;
                }
            } // using

            // Update the comparison item so that the change is reflected in the
            // schema comparison view grid.
            orig = stmt;

            // Update the schema dictionary with the updated version
            schema[SchemaObject.Index][stmt.ObjectName.ToString().ToLower()] = stmt;
        }

        /// <summary>
        /// Update the trigger schema in the specified database file with the updated trigger schema
        /// found in the <paramref name="sql"/>sql parameter.
        /// </summary>
        /// <param name="dbpath">The path to the database where the trigger should be updated</param>
        /// <param name="schema">The schema dictionary of that database file. This is needed because
        /// we want to update the dictionary trigger statement after successfully completing the update.</param>
        /// <param name="sql">The updated SQL string needed in order to re-create the view</param>
        /// <param name="orig">The original view DDL statement object. If everything goes OK, this
        /// will be replaced by the updated DDL statement object.</param>
        private void UpdateTriggerSchema(string dbpath,
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> schema,
            string sql, ref SQLiteDdlStatement orig)            
        {
            sql = Utils.StripComments(sql);
            _scanner.SetSource(sql, 0);

            // Request the parser to parse the SQL statement
            bool ok = _parser.Parse();
            if (!ok)
                throw new ApplicationException("schema has incorrect syntax");

            // Minimal sanity checking
            SQLiteDdlStatement stmt = SQLiteDdlMain.GetStatement();
            if (!(stmt is SQLiteCreateTriggerStatement))
                throw new ApplicationException("schema is not a CREATE TRIGGER statement");
            if (!stmt.ObjectName.Equals(orig.ObjectName))
                throw new ApplicationException("the trigger name is incorrect");

            // Open connection to the database, drop the existing trigger object and
            // re-create it using the updated sql schema
            using (SQLiteConnection conn = Utils.MakeDbConnection(dbpath, true))
            {
                conn.Open();

                SQLiteTransaction tx = conn.BeginTransaction();
                try
                {
                    // First drop the trigger
                    SQLiteCommand drop = new SQLiteCommand(
                        @"DROP TRIGGER " + stmt.ObjectName.ToString(), conn, tx);
                    drop.ExecuteNonQuery();

                    // Then re-create it from scratch
                    SQLiteCommand create = new SQLiteCommand(stmt.ToString(), conn, tx);
                    create.ExecuteNonQuery();

                    tx.Commit();
                }
                catch (SQLiteException se)
                {
                    tx.Rollback();
                    throw;
                }
            } // using

            // Update the comparison item so that the change is reflected in the
            // schema comparison view grid.
            orig = stmt;

            // Update the schema dictionary with the updated version
            schema[SchemaObject.Trigger][stmt.ObjectName.ToString().ToLower()] = stmt;
        }

        /// <summary>
        /// Update the view schema in the specified database file with the updated view schema
        /// found in the <paramref name="sql"/>sql parameter.
        /// </summary>
        /// <param name="dbpath">The path to the database where the view should be updated</param>
        /// <param name="schema">The schema dictionary of that database file. This is needed because
        /// during the view update process we drop the view and any associated triggers need to be
        /// re-created. The schema dictionary contains their information.</param>
        /// <param name="sql">The updated SQL string needed in order to re-create the view</param>
        /// <param name="orig">The original view DDL statement object. If everything goes OK, this
        /// will be replaced by the updated DDL statement object.</param>
        private void UpdateViewSchema(string dbpath, 
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> schema,
            string sql, ref SQLiteDdlStatement orig)
        {
            sql = Utils.StripComments(sql);
            _scanner.SetSource(sql, 0);

            // Request the parser to parse the SQL statement
            bool ok = _parser.Parse();
            if (!ok)
                throw new ApplicationException("schema has incorrect syntax");

            // Minimal sanity checking
            SQLiteDdlStatement stmt = SQLiteDdlMain.GetStatement();
            if (!(stmt is SQLiteCreateViewStatement))
                throw new ApplicationException("schema is not a CREATE VIEW statement");
            if (!stmt.ObjectName.Equals(orig.ObjectName))
                throw new ApplicationException("the view name is incorrect");

            // Open connection to the database, drop th existing view object and
            // re-create it using the updated sql schema
            using (SQLiteConnection conn = Utils.MakeDbConnection(dbpath, true))
            {
                conn.Open();

                SQLiteTransaction tx = conn.BeginTransaction();
                try
                {
                    // First drop the view
                    SQLiteCommand drop = new SQLiteCommand(
                        @"DROP VIEW " + stmt.ObjectName.ToString(), conn, tx);
                    drop.ExecuteNonQuery();

                    // Then re-create it from scratch
                    SQLiteCommand create = new SQLiteCommand(stmt.ToString(), conn, tx);
                    create.ExecuteNonQuery();

                    // When a view is dropped, so are all triggers that are attached to it,
                    // so we have to re-create them as well from scratch.
                    AddObjectTriggers(stmt.ObjectName, schema, conn, tx);

                    tx.Commit();
                }
                catch (SQLiteException se)
                {
                    tx.Rollback();
                    throw;
                }
            } // using

            // Update the comparison item so that the change is reflected in the
            // schema comparison view grid.
            orig = stmt;

            // Update the schema dictionary with the updated version
            schema[SchemaObject.View][stmt.ObjectName.ToString().ToLower()] = stmt;
        }

        /// <summary>
        /// Used to re-create all triggers that belong to the specified view or table. This
        /// is needed because during the update process, all triggers that are
        /// associated with the updated view/table are deleted and need to be re-created
        /// from scratch.
        /// </summary>
        /// <param name="viewName">Name of the view or table</param>
        /// <param name="schema">The target database schema dictionary</param>
        /// <param name="conn">The SQLite connection</param>
        /// <param name="tx">The SQLite transaction</param>
        private void AddObjectTriggers(SQLiteObjectName objName,
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> schema,
            SQLiteConnection conn, SQLiteTransaction tx)
        {
            foreach (SQLiteCreateTriggerStatement trigger in schema[SchemaObject.Trigger].Values)
            {
                if (trigger.TableName.Equals(objName))
                {
                    SQLiteCommand create = new SQLiteCommand(trigger.ToString(), conn, tx);
                    create.ExecuteNonQuery();
                }
            } // foreach
        }
        #endregion

        #region Private Variables
        private SchemaComparisonItem _item;
        private string _leftSQL;
        private string _rightSQL;
        private string _leftdb;
        private string _rightdb;
        private bool _skipNullRows;
        private Scanner _scanner = new Scanner();
        private Parser _parser = new Parser();
        private Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> _leftSchema;
        private Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> _rightSchema;
        private ILog _log = LogManager.GetLogger(typeof(SchemaObjectUpdater));
        #endregion
    }

    /// <summary>
    /// Signals that the update table operation failed
    /// </summary>
    public class UpdateTableException : Exception
    {
        public UpdateTableException(bool restart, string msg)
            : base(msg)
        {
            _canRestart = restart;
        }

        public bool CanRestart
        {
            get { return _canRestart; }
        }

        private bool _canRestart;
    }
}
