using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;
using log4net;
using SQLiteParser;
using Common;

namespace SQLiteTurbo
{
    /// <summary>
    /// This class is responsible to compare table data in two difference databases.
    /// The following conditions are pre-requirements for comparing the two databases:
    /// 1. The two tables have the same set of primary keys (or both have no primary keys at all).
    /// 2. In case the two tables have primary key(s) - there is also a non-empty set of
    ///    columns that are common to both tables (besides the primary key columns). 
    ///    The comparison will be done on these columns.
    /// 3. In case the two tables do not have primary key(s) - there is a non-empty set of
    ///    columns that are common to both tables. The comparison will be done on these columns.
    /// 4. The types of primary key columns must match in both tables
    /// 5. The types of non-primary key columns that are common in both tables must match.
    /// This worker will create a TableDifferences class that will be used to access the 
    /// differences. Table differences will be stored in a temorary database file in order 
    /// to avoid loading them into the main memory. The TableDifferences class is 
    /// responsible to provide an easy to use interface for adding differences and accessing 
    /// differences that are stored in the temporary DB file.
    /// </summary>
    public class TableCompareWorker : IWorker
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TableCompareWorker"/> class.
        /// </summary>
        /// <param name="leftTable">The left table schema</param>
        /// <param name="rightTable">The right table schema</param>
        /// <param name="leftdb">The leftdb.</param>
        /// <param name="rightdb">The rightdb.</param>
        /// <param name="compareBlobs">TRUE will cause the compare worker to compare BLOB fields, FALSE
        /// will cause it to ignore the values stored in BLOB fields. This is mainly for performance reasons
        /// because comparing BLOBs can be very time consuming.</param>
        public TableCompareWorker(SQLiteCreateTableStatement leftTable, 
            SQLiteCreateTableStatement rightTable, string leftdb, string rightdb, bool compareBlobs)
        {
            _leftTable = leftTable;
            _rightTable = rightTable;
            _leftdb = leftdb;
            _rightdb = rightdb;
            _compareBlobs = compareBlobs;

            // Initialize the notification object
            _pevent = new ProgressEventArgs(false, 0, null, null);
        }
        #endregion

        #region IWorker Members

        /// <summary>
        /// Fired whenever the operation has progressed
        /// </summary>
        public event EventHandler<ProgressEventArgs> ProgressChanged;

        /// <summary>
        /// Begin the operation. The method returns immediatly while the operation continues
        /// in the background.
        /// </summary>
        public void BeginWork()
        {
            if (_worker != null && _worker.IsAlive)
                throw new ApplicationException("comparison is already in progress");

            _cancelled = false;
            ThreadStart ts = delegate
            {
                try
                {
                    NotifyPrimaryProgress(false, 0, "Starting to compare table " + _leftTable.ObjectName.ToString() + "...");

                    _result = CompareTable(_leftTable, _rightTable, _leftdb, _rightdb);

                    NotifyPrimaryProgress(true, 100, "Finished comparison");
                }
                catch (UserCancellationException cex)
                {
                    _log.Debug("The user chose to cancel a compare operation");
                    if (_result != null)
                    {
                        _result.Dispose();
                        _result = null;
                    }
                    NotifyPrimaryProgress(true, 100, cex);
                }
                catch (Exception ex)
                {
                    _log.Error("failed to compare databases", ex);
                    if (_result != null)
                    {
                        _result.Dispose();
                        _result = null;
                    }
                    NotifyPrimaryProgress(true, 100, ex);
                } // catch
            };
            _worker = new Thread(ts);
            _worker.IsBackground = true;
            _worker.Name = "TableCompareWorker (" + _leftTable.ObjectName.ToString() + ")";

            _worker.Start();
        }

        /// <summary>
        /// Cancel the operation
        /// </summary>
        public void Cancel()
        {
            _cancelled = true;
        }

        /// <summary>
        /// Get the operation's result (valid only after the work has finished successfully).
        /// </summary>
        /// <value></value>
        public object Result
        {
            get { return _result; }
        }

        /// <summary>
        /// This worker does not supprot dual progress notifications
        /// </summary>
        public bool SupportsDualProgress
        {
            get { return false; }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Notify a primary progress notification
        /// </summary>
        private void NotifyPrimaryProgress(bool done, int progress, string msg)
        {
            _pevent.IsDone = done;
            _pevent.Progress = progress;
            _pevent.Message = msg;
            _pevent.Error = null;

            if (ProgressChanged != null)
                ProgressChanged(this, _pevent);
        }

        /// <summary>
        /// Notify a primary progress error notification
        /// </summary>
        private void NotifyPrimaryProgress(bool done, int progress, Exception error)
        {
            _pevent.IsDone = done;
            _pevent.Progress = progress;
            _pevent.Message = null;
            _pevent.Error = error;

            if (ProgressChanged != null)
                ProgressChanged(this, _pevent);
        }

        /// <summary>
        /// The main method that is responsible for comparing all table rows in both left
        /// and right databases.
        /// </summary>
        /// <param name="leftTable">The left table schema</param>
        /// <param name="rightTable">The right table schema</param>
        /// <param name="leftdb">The path to the left database file.</param>
        /// <param name="rightdb">The path to the right database file.</param>
        /// <returns>
        /// A table-changes object that contains all changes that were found in the search
        /// </returns>
        private TableChanges CompareTable(SQLiteCreateTableStatement leftTable, 
            SQLiteCreateTableStatement rightTable, string leftdb, string rightdb)
        {
            string leftstr = GetConnectionString(leftdb);
            string rightstr = GetConnectionString(rightdb);
            SQLiteObjectName tableName = leftTable.ObjectName;

            // Make sure that the two tables can be compared.
            string errmsg;
            if (!Utils.IsTableComparisonAllowed(leftTable, rightTable, out errmsg, _compareBlobs))
                throw new ApplicationException(errmsg);

            // Get the list of common columns
            List<SQLiteColumnStatement> common = Utils.GetCommonColumns(rightTable, leftTable);

            // Get the list of primary key columns
            List<SQLiteColumnStatement> pkeys = Utils.GetPrimaryColumns(leftTable);

            // Create the table changes object that will hold all difference that will be 
            // found by the comparison algorithm.
            TableChanges res = new TableChanges(leftTable, rightTable, leftdb, rightdb);
            try
            {
                // Instruct the table changes object to enter UPDATE mode
                res.BeginUpdate();

                // In case the primary key(s) of the table contain a BLOB column or if there are no
                // primary key(s) but there is at least a single common BLOB column - we'll need to
                // use a different version of the comparison methods.
                if (Utils.ContainsBlobColumn(pkeys) || (pkeys.Count==0 && Utils.ContainsBlobColumn(common)))
                {
                    // Compare the rows in the left table to the rows in the right database table
                    CompareLeftToRightGeneral(leftTable, rightTable, leftstr, rightstr, res);

                    // Compare the rows in the right database table to the rows in the left database table
                    CompareRightToLeftGeneral(tableName, pkeys, common, leftstr, rightstr, res);
                }
                else
                {
                    // Compare from the left database to the right database.
                    // The changes we can find are:
                    // 1. A row exists in both databases and is different
                    // 2. A row exists in both databases and is the same row
                    // 3. A row exists only in the left database but not in the right database.
                    CompareLeftToRight(tableName, pkeys, common, rightstr, leftstr, res);

                    // Compare from the right database to the left database.
                    // The changes we can find are:
                    // 1. A row exists in the right database but not in the left database.
                    CompareRightToLeft(tableName, pkeys, common, leftstr, rightstr, res);
                } // else

                // Instruct the table changes object to exit UPDATE mode
                res.EndUpdate();
            }
            catch(Exception ex)
            {
                res.Dispose();
                throw;
            } // finally

            return res;
        }

        /// <summary>
        /// Traverse the entire right database table and look for rows that exist
        /// only in the right database but not in the left database. Add the appropriate
        /// difference item into the table changes object.
        /// </summary>
        /// <param name="table">The schema object of the compared table</param>
        /// <param name="leftstr">The connection string to the left database.</param>
        /// <param name="rightstr">The connection string to the right database.</param>
        /// <param name="res">The table changes object into which the method will
        /// write any differences it finds.</param>
        private void CompareRightToLeft(SQLiteObjectName tableName,
            List<SQLiteColumnStatement> pkeys,
            List<SQLiteColumnStatement> common,
            string leftstr, string rightstr, TableChanges res)
        {
            // Open a connection to the right database
            using (SQLiteConnection right = new SQLiteConnection(rightstr))
            {
                right.Open();

                using (SQLiteConnection left = new SQLiteConnection(leftstr))
                {
                    left.Open();

                    // Extract the list of primary keys from the table statement
                    List<SQLiteColumnStatement> ckeys = null;
                    if (pkeys.Count > 0)
                    {
                        // In case the table has primary key(s) we'll use these keys to locate
                        // a matching row in the left database table.
                        ckeys = pkeys;
                    }
                    else
                    {
                        // In case the table does not have primary key(s) we'll use the entire
                        // set of table columns to match rows in the left database table.
                        ckeys = common;
                    } // else

                    SQLiteTransaction rightTx = right.BeginTransaction(true);
                    SQLiteTransaction leftTx = left.BeginTransaction(true);
                    try
                    {
                        string tblname = tableName.ToString();

                        // Build the command that will be used to check the total number of
                        // rows we need to check (for progress notifications).
                        SQLiteCommand scount = new SQLiteCommand(
                            @"SELECT COUNT(*) FROM " + tblname, right, rightTx);
                        long total = (long)scount.ExecuteScalar();

                        // Iterate over all rows in the right database table and try to match every 
                        // row to a row in the left database table (done in the 'find' command)
                        SQLiteCommand select = BuildSelectCommand(tableName.ToString(), common, right, rightTx);
                        using (SQLiteDataReader reader = select.ExecuteReader())
                        {
                            int prev = -1;
                            long offset = 0;
                            while (reader.Read())
                            {
                                // Create the FIND command that will try to locate the matching row in
                                // the left database.
                                SQLiteCommand find = BuildFindCommand(tableName, ckeys, common, reader, left, leftTx, true, true);

                                long count = (long)find.ExecuteScalar();
                                if (count == 0)
                                {
                                    long rowId = (long)reader[0];
                                    res.AddExistsInRight(rowId);
                                }

                                offset++;
                                double progress = 50.0 + 50.0 * offset / total;
                                if ((int)progress != prev)
                                {
                                    prev = (int)progress;
                                    NotifyPrimaryProgress(false, prev, tblname + " " + offset + " rows compared so far (" + prev + "% done)");
                                }

                                if (_cancelled)
                                    throw new UserCancellationException();
                            } // while
                        } // using

                        rightTx.Commit();

                    }
                    catch (Exception ex)
                    {
                        rightTx.Rollback();
                        throw;
                    } // catch
                } // using
            } // using
        }

        /// <summary>
        /// This method is used to compare the rows in the left table to the rows in the
        /// right table. It should only be used in cases where BLOBs are part of the primary
        /// key of the table or when there is no primary key but there are BLOB columns.
        /// </summary>
        /// <remarks>The reason that this method should not be used in the general case is
        /// because it loads the entire BLOB column fields into memory. I went to great lengths
        /// to avoid this whenever possible, but the case when a table's primary key(s) contain
        /// a BLOB column or when a table doesnt have a primary key but do have BLOB column(s) is
        /// an exception because it forces us to load BLOB fields into memory.</remarks>
        /// <param name="leftTable">The schema of the left table</param>
        /// <param name="rightTable">The schema of the right table</param>
        /// <param name="leftstr">The connection string to the left database</param>
        /// <param name="rightstr">The connection string to the right database</param>
        /// <param name="changes">The table changes object that will contain the results of the comparison</param>
        private void CompareLeftToRightGeneral(
            SQLiteCreateTableStatement leftTable,
            SQLiteCreateTableStatement rightTable,
            string leftstr, string rightstr, TableChanges changes)
        {
            string tname = leftTable.ObjectName.ToString();
            using (SQLiteConnection left = new SQLiteConnection(leftstr))
            {
                left.Open();
                using (SQLiteConnection right = new SQLiteConnection(rightstr))
                {
                    right.Open();

                    SQLiteTransaction leftTx = left.BeginTransaction(true);
                    SQLiteTransaction rightTx = right.BeginTransaction(true);

                    try
                    {
                        // This command is used to evaluate operation progress
                        SQLiteCommand scount = new SQLiteCommand(
                            @"SELECT COUNT(*) FROM " + tname, left, leftTx);
                        long total = (long)scount.ExecuteScalar();

                        // Compute the list of common columns and the list of columns that are common but have different
                        // types in both databases
                        List<SQLiteColumnStatement> dcols = null;
                        List<SQLiteColumnStatement> common = Utils.GetCommonColumns(leftTable, rightTable, false, out dcols);

                        // Used for optimizing search
                        bool hasBlobColumn = Utils.ContainsBlobColumn(common);

                        // Compute the list of primary table keys
                        List<SQLiteColumnStatement> pkeys = Utils.GetPrimaryColumns(leftTable);

                        // If there are no primary keys - use all common columns for doing the comparison.
                        bool haspkey;
                        if (pkeys.Count == 0)
                        {
                            pkeys = common;
                            haspkey = false;
                        }
                        else
                            haspkey = true;

                        // This command will be used to iterate over the entire table
                        // in the left database.
                        string clist = Utils.BuildColumnsString(common, false);
                        SQLiteCommand select = new SQLiteCommand(@"SELECT RowID, " + clist + " FROM " +
                            tname, left, leftTx);

                        // This command will be used to match a row in the left database table with a row
                        // in the right database table.
                        SQLiteCommand find = null;

                        // Start iterating over the left database table
                        long rightRowId;
                        using (SQLiteDataReader reader = select.ExecuteReader())
                        {
                            long offset = 0;
                            int prev = -1;
                            while (reader.Read())
                            {
                                long leftRowId = (long)reader["RowID"];

                                // Build the find command necessary for finding the matching row in the right database.
                                find = BuildFindCommand(leftTable.ObjectName, pkeys, common, reader, right, rightTx, false, false);

                                // Try to find a match and compare the rows
                                CompareRowsGeneral(find, haspkey, leftRowId, reader, common, hasBlobColumn, changes);

                                // Evaluate progress
                                offset++;
                                double progress = 50.0 * offset / total;
                                if ((int)progress != prev)
                                {
                                    prev = (int)progress;
                                    NotifyPrimaryProgress(false, prev, tname + " " + offset + " rows compared so far (" + prev + "% done)");
                                }

                                if (_cancelled)
                                    throw new UserCancellationException();
                            } // while
                        } // using

                        leftTx.Commit();
                        rightTx.Commit();
                    }
                    catch (Exception ex)
                    {
                        leftTx.Rollback();
                        rightTx.Rollback();
                        throw;
                    } // catch
                } // using
            } // using

        }
        
        /// <summary>
        /// This method should be used when comparing general rows that contain in-memory
        /// BLOB fields. It should only be called from CompareLeftToRightGeneral() method.
        /// </summary>
        /// <param name="find">The command to use for matching a right table row</param>
        /// <param name="haspkey">TRUE to indicate that the table has primary key(s), FALSE to indicate
        /// that there are no primary key(s) in the table.</param>
        /// <param name="leftRowId">The ID of the left table row</param>
        /// <param name="reader">The data reader object (into the right datbase table)</param>
        /// <param name="changes">The table changes object to use for inserting the results of the comparison</param>
        private void CompareRowsGeneral(SQLiteCommand find, bool haspkey,
            long leftRowId, SQLiteDataReader reader, List<SQLiteColumnStatement> common, 
            bool hasBlobColumn,
            TableChanges changes)
        {
            // Execute the command and look for a matching row
            long rightRowId = -1;
            using (SQLiteDataReader rightReader = find.ExecuteReader())
            {
                if (!rightReader.HasRows)
                {
                    rightRowId = -1;
                    changes.AddExistsInLeft(leftRowId);
                }
                else
                {
                    // There should be at least one row in the right database table. 
                    // There will be exactly one row if the table has a primary key or 
                    // potentially more than one row if the table does not have a primary key.
                    while (rightReader.Read())
                    {
                        // Found the matching row's ROW-ID value
                        rightRowId = (long)rightReader[0];

                        // If the table does not have a primary key - check in the differences
                        // database to make sure that this row was not already matched with
                        // a row from the left database table.
                        // Note: this step makes the comparison process more IO intensive so I
                        //       make sure to invoke it only when comparing tables that don't
                        //       have primary key(s).
                        if (!haspkey)
                        {
                            // If the differences database has a match for the right row - 
                            // continue to scan the right database table until a row has no
                            // match in the differences database or until all rows in the right
                            // database table were scanned (in which case we'll add a EXISTS_IN_LEFT
                            // comparison item to the differences database).
                            if (changes.HasRightRowId(TableChanges.SAME_ROWS_TABLE_NAME, rightRowId))
                                continue;
                        }

                        // Comparing the actual rows values is relevant only when the table has a primary
                        // key. Otherwise - the match is complete (all columns need to match because the
                        // row's identity is composed from all of its columns).
                        if (haspkey)
                        {
                            // Now check whether all values in both table rows are the same
                            bool hasdiff = false;
                            List<string> changedBlobs = null;
                            foreach (SQLiteColumnStatement col in common)
                            {
                                string colname = SQLiteParser.Utils.Chop(col.ObjectName.ToString());

                                object lval = reader[colname];
                                object rval = rightReader[colname];
                                if (!Utils.ObjectsAreEqual(lval, rval))
                                {
                                    if (lval is byte[] || rval is byte[])
                                    {
                                        if (changedBlobs == null)
                                            changedBlobs = new List<string>();
                                        if (!changedBlobs.Contains(colname))
                                            changedBlobs.Add(colname);
                                    }
                                    else
                                    {
                                        hasdiff = true;
                                        if (!hasBlobColumn)
                                            break;
                                    } // else
                                } // if
                            } // foreach

                            if (hasdiff || changedBlobs != null)
                            {
                                // Add a difference row for this row
                                changes.AddDifferent(leftRowId, rightRowId, changedBlobs);
                                return;
                            } // if
                        } // if

                        // Both rows have the same data
                        changes.AddSame(leftRowId, rightRowId);
                        return;
                    } // while

                    // If we've reached here - it means that we've scanned the right database
                    // table rows that are the same as the row in the left database table and
                    // all these rows (there are more than 0 such rows) have already been matched
                    // to rows in the left database table, so we have to conclude that the left database
                    // table row does not match any row in the right database table.
                    changes.AddExistsInLeft(leftRowId);
                } // else
            } // using                                
        }

        /// <summary>
        /// Compares rows from the right database table with rows from the left database table. 
        /// Should be used only when doing general table comparisons that involve BLOB columns
        /// in tables with primary key(s) that contain at least a single primary key BLOB column or tables
        /// that don't have primary key(s) but have at least one BLOB column.
        /// </summary>
        /// <param name="tableName">The name of the table to compare</param>
        /// <param name="pkeys">The list of primary keys</param>
        /// <param name="common">The list of columns that are common to the tables in both databases</param>
        /// <param name="leftstr">The connection string to the left database file</param>
        /// <param name="rightstr">The connection string to the right database file</param>
        /// <param name="res">The table changes object that will contain the results of the comparison</param>
        private void CompareRightToLeftGeneral(SQLiteObjectName tableName,
            List<SQLiteColumnStatement> pkeys,
            List<SQLiteColumnStatement> common,
            string leftstr, string rightstr, TableChanges res)
        {
            // Open a connection to the right database
            using (SQLiteConnection right = new SQLiteConnection(rightstr))
            {
                right.Open();

                using (SQLiteConnection left = new SQLiteConnection(leftstr))
                {
                    left.Open();

                    // Extract the list of primary keys from the table statement
                    List<SQLiteColumnStatement> ckeys = null;
                    if (pkeys.Count > 0)
                    {
                        // In case the table has primary key(s) we'll use these keys to locate
                        // a matching row in the left database table.
                        ckeys = pkeys;
                    }
                    else
                    {
                        // In case the table does not have primary key(s) we'll use the entire
                        // set of table columns to match rows in the left database table.
                        ckeys = common;
                    } // else

                    SQLiteTransaction rightTx = right.BeginTransaction(true);
                    SQLiteTransaction leftTx = left.BeginTransaction(true);
                    try
                    {
                        string tblname = tableName.ToString();

                        // Build the command that will be used to check the total number of
                        // rows we need to check (for progress notifications).
                        SQLiteCommand scount = new SQLiteCommand(
                            @"SELECT COUNT(*) FROM " + tblname, right, rightTx);
                        long total = (long)scount.ExecuteScalar();

                        // Build the SELECT command for looping through all rows in the right database table.
                        SQLiteCommand select = new SQLiteCommand(
                            @"SELECT RowID," + Utils.BuildColumnsString(common, false) + " FROM " + tblname, right, rightTx);

                        // Iterate over all rows in the right database table and try to match every 
                        // row to a row in the left database table (done in the 'find' command)
                        using (SQLiteDataReader reader = select.ExecuteReader())
                        {
                            int prev = -1;
                            long offset = 0;
                            while (reader.Read())
                            {
                                // Create the FIND command that will try to locate the matching row in
                                // the left database.
                                SQLiteCommand find = BuildFindCommand(tableName, ckeys, common, reader, left, leftTx, true, false);

                                long count = (long)find.ExecuteScalar();
                                if (count == 0)
                                {
                                    long rowId = (long)reader[0];
                                    res.AddExistsInRight(rowId);
                                }

                                offset++;
                                double progress = 50.0 + 50.0 * offset / total;
                                if ((int)progress != prev)
                                {
                                    prev = (int)progress;
                                    NotifyPrimaryProgress(false, prev, tblname + " " + offset + " rows compared so far (" + prev + "% done)");
                                }

                                if (_cancelled)
                                    throw new UserCancellationException();
                            } // while
                        } // using

                        rightTx.Commit();
                    }
                    catch (Exception ex)
                    {
                        rightTx.Rollback();
                        throw;
                    } // catch
                } // using
            } // using
        }

        /// <summary>
        /// Traverse the entire left database table. For every row there - try to find it
        /// in the right database table. There can be one of 3 results:
        /// <list>
        /// 		<item>The same row exists in the right database and has the same data</item>
        /// 		<item>The same row exists in the right database and has different data</item>
        /// 		<item>The same row does not exist in the right database</item>
        /// 	</list>
        /// The method updates all these results into the difference database.
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <param name="common">A list of columns that are common to both left and right tables</param>
        /// <param name="rightstr">The connection string to the right database</param>
        /// <param name="leftstr">The connection string to the left database</param>
        /// <param name="res">The results table-changes object into which all difference
        /// items will be written by this method.</param>
        private void CompareLeftToRight(SQLiteObjectName tableName, 
            List<SQLiteColumnStatement> pkeys,
            List<SQLiteColumnStatement> common, 
            string rightstr, string leftstr, TableChanges res)
        {
            string tname = tableName.ToString();
            using (SQLiteConnection left = new SQLiteConnection(leftstr))
            {
                left.Open();
                using (SQLiteConnection right = new SQLiteConnection(rightstr))
                {
                    right.Open();

                    SQLiteTransaction leftTx = left.BeginTransaction(true);
                    SQLiteTransaction rightTx = right.BeginTransaction(true);

                    BlobReaderWriter brw = null;
                    try
                    {
                        // This command is used to evaluate operation progress
                        SQLiteCommand scount = new SQLiteCommand(
                            @"SELECT COUNT(*) FROM " + tname, left, leftTx);
                        long total = (long)scount.ExecuteScalar();

                        // This command will be used to iterate over the entire table
                        // in the left database.
                        List<SQLiteColumnStatement> leftCommon = Utils.GetMatchingColumns(_leftTable.Columns, common);
                        SQLiteCommand select = BuildSelectCommand(tname, leftCommon, left, leftTx);

                        // Construct a BLOB reader/writer object for comparing BLOBs if necessary                        
                        bool hasBlobColumns = Utils.ContainsBlobColumn(leftCommon);
                        if (_compareBlobs && hasBlobColumns)
                            brw = new BlobReaderWriter(_leftdb, _rightdb);

                        // Start iterating over the left database table
                        using (SQLiteDataReader reader = select.ExecuteReader())
                        {
                            long offset = 0;
                            int prev = -1;
                            while (reader.Read())
                            {
                                // For every row - try to locate the row with same primary key in the 
                                // right database and give a comparison result.
                                CompareRows(tableName, res, pkeys, common, reader, right, rightTx, brw, hasBlobColumns);

                                // Evaluate progress
                                offset++;
                                double progress = 50.0 * offset / total;
                                if ((int)progress != prev)
                                {
                                    prev = (int)progress;
                                    NotifyPrimaryProgress(false, prev, tname + " " + offset + " rows compared so far (" + prev + "% done)");
                                }

                                if (_cancelled)
                                    throw new UserCancellationException();
                            } // while
                        } // using

                        leftTx.Commit();
                        rightTx.Commit();
                    }
                    catch (Exception ex)
                    {
                        leftTx.Rollback();
                        rightTx.Rollback();
                        throw;
                    } // catch
                    finally
                    {
                        // Dispose of the BlobReaderWriter object if necessary
                        if (brw != null)
                            brw.Dispose();
                    }
                } // using
            } // using
        }

        /// <summary>
        /// Builds a SELECT statement that will provide the rows for comparison between
        /// the two databases.
        /// </summary>
        /// <param name="tableName">Name of the table in which SELECT will be performed</param>
        /// <param name="common">A list of columns that are common to both left and right tables</param>
        /// <param name="conn">The SQLite connection</param>
        /// <param name="tx">The SQLite transaction</param>
        /// <returns>A command that can be used to perform SELECT</returns>
        private SQLiteCommand BuildSelectCommand(string tableName, 
            List<SQLiteColumnStatement> common, SQLiteConnection conn, 
            SQLiteTransaction tx)
        {
            string clist = Utils.BuildColumnsString(common, true);
            SQLiteCommand select = new SQLiteCommand(@"SELECT RowID, "+clist+" FROM " +
                tableName, conn, tx);
            return select;
        }

        /// <summary>
        /// Compares a row that exists in the left database to a matching row that
        /// exists in the right database (by matching we mean that the rows have the
        /// same primary key).
        /// </summary>
        /// <param name="common">A list of common columns that are used for the comparison</param>
        /// <param name="leftReader">The reader to the left database</param>
        /// <param name="leftRowId">The RowID of the row in the left database.</param>
        /// <param name="rightRowId">The RowID of the matching row in the right database (or
        /// -1 if no such raw was found).</param>
        /// <returns>The comparison result</returns>
        private void CompareRows(
            SQLiteObjectName tableName,
            TableChanges changes,
            List<SQLiteColumnStatement> pkeys,
            List<SQLiteColumnStatement> common,
            SQLiteDataReader leftReader,
            SQLiteConnection right,
            SQLiteTransaction rightTx, 
            BlobReaderWriter brw,
            bool hasBlobColumns)
        {
            long rightRowId = -1;
            long leftRowId = (long)leftReader[0];

            // Populate the find command parameters
            SQLiteCommand find;
            bool haspkey = false;
            if (pkeys.Count > 0)
            {
                haspkey = true;
                find = BuildFindCommand(tableName, pkeys, common, leftReader, right, rightTx, false, true);
            }
            else
            {
                // When there are no primary key columns - we'll use the common columns as a replacement
                // for the primary keys (we'll search based on all of them).
                find = BuildFindCommand(tableName, common, common, leftReader, right, rightTx, false, true);
            }

            // Execute the command and look for a matching row
            using (SQLiteDataReader rightReader = find.ExecuteReader())
            {
                if (!rightReader.HasRows)
                {
                    rightRowId = -1;
                    changes.AddExistsInLeft(leftRowId);
                    return;
                }

                // There should be at least one row in the right database table. 
                // There will be exactly one row if the table has a primary key or 
                // potentially more than one row if the table does not have a primary key.
                while (rightReader.Read())
                {
                    // Found the matching row's ROW-ID value
                    rightRowId = (long)rightReader[0];

                    // If the table does not have a primary key - check in the differences
                    // database to make sure that this row was not already matched with
                    // a row from the left database table.
                    // Note: this step makes the comparison process more IO intensive so I
                    //       make sure to invoke it only when comparing tables that don't
                    //       have primary key(s).
                    if (!haspkey)
                    {
                        // If the differences database has a match for the right row - 
                        // continue to scan the right database table until a row has no
                        // match in the differences database or until all rows in the right
                        // database table were scanned (in which case we'll add a EXISTS_IN_LEFT
                        // comparison item to the differences database).
                        if (changes.HasRightRowId(TableChanges.SAME_ROWS_TABLE_NAME, rightRowId))
                            continue;
                    }

                    // Comparing the actual rows values is relevant only when the table has a primary
                    // key. Otherwise - the match is complete (all columns need to match because the
                    // row's identity is composed from all of its columns).
                    if (haspkey)
                    {
                        // Now check whether all values in both table rows are the same
                        bool hasdiff = false;
                        List<string> changedBlobs = null;
                        foreach (SQLiteColumnStatement col in common)
                        {
                            string colname = SQLiteParser.Utils.Chop(col.ObjectName.ToString());

                            // Blobs need special treatment:
                            // In order to avoid loading potentially huge BLOB data arrays into memory
                            // for comparison I've changed the SELECT command to return the result if IS NULL of 
                            // those BLOB columns instead of their actual values. At this point we can
                            // do simple comparison based on the columns nullability.
                            // If both fields are NULL - then they are equal
                            // If one of the fields is NULL and the other is not - then they are not equal.
                            // If both fields are not NULL - we'll have to actually compare their contents
                            // byte by byte (if BLOB comparison was requested). This is done by
                            // a special class that performs the comparison using a streaming procedure
                            // so that these BLOBs are never loaded into main memory in their entirety.

                            // Another note: When BLOBs are compared - we have to continue their comparison
                            // until all BLOB columns are compared in the two rows. This is needed even
                            // though we could quit the comparison process after the first difference
                            // is found. The reason we don't quit the comparison process is because we
                            // still want the GUI to show BLOB differences and this can't be done at the
                            // GUI level (like it is done for all other normal types that are loaded into
                            // main memory in their entirety - which allows these comparisons
                            // to take place in the GUI).

                            // One last note: Sometimes the same column may be declared as a BLOB in one
                            // of the databases and as something else in the other database. In such cases we don't
                            // compare at all - we'll simply assume that the two columns are not equal.
                            SQLiteColumnStatement leftCol = Utils.GetColumnByName(_leftTable, colname);
                            SQLiteColumnStatement rightCol = Utils.GetColumnByName(_rightTable, colname);
                            if (Utils.IsColumnTypesMatching(leftCol, rightCol))
                            {
                                if (Utils.GetDbType(col.ColumnType) == System.Data.DbType.Binary)
                                {
                                    long lb = (long)leftReader[colname];
                                    long rb = (long)rightReader[colname];

                                    // Easy test: check nullability of both fields
                                    if (lb == 1 && rb == 0 ||
                                        lb == 0 && rb == 1) // One of the BLOBs is null and the other is not
                                    {
                                        if (changedBlobs == null)
                                            changedBlobs = new List<string>();
                                        changedBlobs.Add(col.ObjectName.ToString());
                                    }
                                    else if (lb == 1 && rb == 1) // Both BLOBS are not NULL
                                    {
                                        // In this case we'll have to actually compare the contents of
                                        // the two BLOB fields in order to check if the two BLOBs are the same,
                                        // but we'll do this only if we are instructed to compare BLOB fields.
                                        if (brw != null)
                                        {
                                            // Note: This code can execute for long time (depending on the size 
                                            // of the BLOBs compared), however - it should never cause the process
                                            // to crash due to memory constraints!
                                            bool equal =
                                                brw.CompareBlobs(SQLiteParser.Utils.Chop(tableName.ToString()).ToLower(),
                                                SQLiteParser.Utils.Chop(col.ObjectName.ToString()).ToLower(),
                                                leftRowId, rightRowId, BlobComparisonListener);
                                            if (!equal)
                                            {
                                                if (changedBlobs == null)
                                                    changedBlobs = new List<string>();
                                                changedBlobs.Add(col.ObjectName.ToString());
                                            } // if
                                        } // if
                                    } // else
                                }
                                else if (!leftReader[colname].Equals(rightReader[colname]))
                                {
                                    // Rows are different in at least one column value.
                                    hasdiff = true;

                                    if (!hasBlobColumns)
                                    {
                                        // As long as the compared columns do not include BLOB columns
                                        // we can safely break from the comparison loop here because
                                        // there are no BLOB columns that need to be compared.
                                        break;
                                    }
                                } // else
                            } // if
                            else
                            {
                                DbType ltype = Utils.GetDbType(leftCol.ColumnType);
                                DbType rtype = Utils.GetDbType(rightCol.ColumnType);
                                object lval = leftReader[colname];
                                object rval = rightReader[colname];
                                if (ltype == DbType.Binary && lval is long && ((long)lval) == 0)
                                    lval = DBNull.Value;
                                if (rtype == DbType.Binary && rval is long && ((long)rval) == 0)
                                    rval = DBNull.Value;                                    

                                // Basically - if the two columns have mismatching types - they are not equal. There is 
                                // one exception though - when both values are NULL.
                                if (lval != DBNull.Value || rval != DBNull.Value)
                                {
                                    // The two columns have mismatching types - so they are not equal. If one of them
                                    // is a BLOB - mark the fact that the two columns are not equal in the changedBlobs list.
                                    if (ltype == DbType.Binary || rtype == DbType.Binary)
                                    {
                                        if (changedBlobs == null)
                                            changedBlobs = new List<string>();
                                        changedBlobs.Add(col.ObjectName.ToString());
                                    }
                                    else
                                    {
                                        hasdiff = true;

                                        if (!hasBlobColumns)
                                        {
                                            // As long as the compared columns do not include BLOB columns
                                            // we can safely break from the comparison loop here because
                                            // there are no BLOB columns that need to be compared.
                                            break;
                                        }
                                    } // else
                                } // if
                            } // else
                        } // foreach

                        if (hasdiff || changedBlobs != null)
                        {
                            // Add a difference row for this row
                            changes.AddDifferent(leftRowId, rightRowId, changedBlobs);
                            return;
                        }
                    } // if

                    // Both rows have the same data
                    changes.AddSame(leftRowId, rightRowId);

                    return;
                } // while

                // If we've reached here - it means that we've scanned the right database
                // table rows that are the same as the row in the left database table and
                // all these rows (there are more than 0 such rows) have already been matched
                // to rows in the left database table, so we have to conclude that the left database
                // table row does not match any row in the right database table.
                changes.AddExistsInLeft(leftRowId);
            } // using
        }

        /// <summary>
        /// Used to hook into the BLOB comparison process and allow the user to cancel
        /// the operation.
        /// </summary>
        private void BlobComparisonListener(int bytesRead, int totalBytes, ref bool cancel)
        {
            // Allow the user to cancel even in the middle of BLOB comparison because these
            // may take an inordinate amount of time to complete.
            cancel = _cancelled;
        }

        /// <summary>
        /// DEBUG
        /// </summary>
        private void DumpCommand(SQLiteCommand cmd)
        {
            _log.Debug("SQL-COMMAND: " + cmd.CommandText);
            foreach (SQLiteParameter p in cmd.Parameters)
            {
                string valstr = "NULL";
                if (p.Value != DBNull.Value)
                {
                    valstr = p.Value.ToString();
                    if (valstr == string.Empty)
                        System.Diagnostics.Debugger.Break();
                }
                _log.Debug("PARAM [" + p.ParameterName + "]: " + valstr);
            } // foreach
        }

        /// <summary>
        /// Fill WHERE parameters with values needed for doing the search
        /// </summary>
        /// <param name="tableName">The name of the table that is compared</param>
        /// <param name="pkeys">the list of column keys. In case the table have actual primary keys - these
        /// will be in this parameter, otherwise - when the table has no primary key(s) - this list will contain
        /// the list of columns that are common to both tables (left and right)</param>
        /// <param name="common">The list of columns that are common to both tables (left and right)</param>
        /// <param name="reader">The reader from which the values to the WHERE parameters are taken</param>
        /// <param name="conn">The SQLIte connection</param>
        /// <param name="tx">The transaction object</param>
        /// <param name="asCount">TRUE means that we'll return a command that counts the number of rows, FALSE
        /// means that we'll return a command that returns the actual rows that match the search criteria</param>
        private static SQLiteCommand BuildFindCommand(
            SQLiteObjectName tableName,
            List<SQLiteColumnStatement> pkeys,
            List<SQLiteColumnStatement> common, 
            SQLiteDataReader reader,
            SQLiteConnection conn,
            SQLiteTransaction tx,
            bool asCount,
            bool castBlobs)
        {
            SQLiteCommand res = new SQLiteCommand();
            res.Connection = conn;
            res.Transaction = tx;

            StringBuilder sb = new StringBuilder();
            if (asCount)
                sb.Append("SELECT COUNT(*) FROM " + tableName.ToString() + " WHERE ");
            else
            {
                string clist = Utils.BuildColumnsString(common, castBlobs);
                sb.Append("SELECT RowID, "+clist+" FROM " + tableName.ToString() + " WHERE ");
            }

            for(int i=0; i<pkeys.Count; i++)
            {
                SQLiteColumnStatement col = pkeys[i];

                string colname = col.ObjectName.ToString();
                object value = reader[SQLiteParser.Utils.Chop(colname)];

                // Note: This is a special handling for null values. When comparing 
                //       NULL value we should always use 'IS NULL' and not '= NULL' otherwise
                //       we'll have wrong comparison results.
                if (object.ReferenceEquals(value, DBNull.Value))
                    sb.Append(colname + " IS NULL");
                else
                {
                    string prmname = Utils.GetColumnParameterName(colname);

                    // Needed in order to handle weak support for comparing dates in SQLite
                    if (Utils.GetDbType(col.ColumnType) == DbType.DateTime)
                        sb.Append("DATETIME(" + colname + ") = DATETIME(" + prmname+")");
                    else
                        sb.Append(colname + " = " + prmname);

                    DbType dbtype = Utils.GetDbTypeFromClrType(value);
                    res.Parameters.Add(prmname, dbtype);
                    res.Parameters[prmname].Value = value;
                } // else

                if (i < pkeys.Count - 1)
                    sb.Append(" AND ");
            } // foreach

            res.CommandText = sb.ToString();
            return res;
        }

        /// <summary>
        /// Returns a connection string to the specified SQLite datbase file (READ-ONLY connection).
        /// </summary>
        /// <param name="fpath">The path to the database file</param>
        /// <returns>A connection string to that file.</returns>
        private string GetConnectionString(string fpath)
        {
            SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder();
            sb.DataSource = fpath;
            //sb.UseUTF16Encoding = true;
            sb.ReadOnly = true;

            return sb.ConnectionString;
        }
        #endregion

        #region Private Variables
        private string _leftdb;
        private string _rightdb;
        private ProgressEventArgs _pevent;
        private SQLiteCreateTableStatement _leftTable;
        private SQLiteCreateTableStatement _rightTable;
        private bool _cancelled;
        private bool _compareBlobs = false;
        private Thread _worker;
        private TableChanges _result;
        private ILog _log = LogManager.GetLogger(typeof(TableCompareWorker));
        #endregion
    }
}
