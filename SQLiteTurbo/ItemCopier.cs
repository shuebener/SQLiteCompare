using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using SQLiteParser;
using log4net;
using Common;

namespace SQLiteTurbo
{
    /// <summary>
    /// This class contains the DB merge operations that are used
    /// in the application (e.g., copy from left DB to right DB etc).
    /// </summary>
    public class ItemCopier : IWorker
    {
        #region Constructors
        /// <summary>
        /// Copies the DB entity specified in the comparison item from left to right
        /// or vice versa (depending on the value of the leftToRight parameter).
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="item">The comparison item</param>
        /// <param name="leftdb">The path to the left database file</param>
        /// <param name="rightdb">The path to the right database file</param>
        /// <param name="leftToRight">TRUE means copying will be done from the left database
        /// to the right database, FALSE means copying will be done from the right database
        /// to the left database.</param>
        public ItemCopier(Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> leftSchema,
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> rightSchema, 
            SchemaComparisonItem item, string leftdb, string rightdb, bool leftToRight)
        {
            _leftSchema = leftSchema;
            _rightSchema = rightSchema;
            _item = item;
            _leftdb = leftdb;
            _rightdb = rightdb;
            _leftToRight = leftToRight;
            _name = item.ObjectName;

            _pevent = new ProgressEventArgs(false, 0, null, null);
        }
        #endregion

        #region IWorker Members

        /// <summary>
        /// Fired whenever the operation has progressed
        /// </summary>
        public event EventHandler<ProgressEventArgs> ProgressChanged;

        /// <summary>
        /// Begin the operation
        /// </summary>
        public void BeginWork()
        {
            if (_worker != null && _worker.IsAlive)
                throw new ApplicationException("copying is already in progress");

            _cancelled = false;
            ThreadStart ts = delegate
            {
                try
                {
                    NotifyPrimaryProgress(false, 0, "Starting to copy item " + _name + "...");

                    CopyItem();

                    NotifyPrimaryProgress(true, 100, "Finished copying");
                }
                catch (UserCancellationException cex)
                {
                    _log.Debug("The user chose to cancel the operation");
                    NotifyPrimaryProgress(true, 100, cex);
                }
                catch (Exception ex)
                {
                    _log.Error("failed to copy item", ex);
                    NotifyPrimaryProgress(true, 100, ex);
                } // catch
            };
            _worker = new Thread(ts);
            _worker.IsBackground = true;
            _worker.Name = "ItemCopier (" + _name + ")";

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
            get { return null; }
        }

        /// <summary>
        /// This worker does not support dual progress notifications
        /// </summary>
        public bool SupportsDualProgress
        {
            get { return false; }
        }

        #endregion


        #region Private Methods

        private void CopyItem()
        {
            if (_item.LeftDdlStatement != null && _item.LeftDdlStatement is SQLiteCreateTableStatement ||
                _item.RightDdlStatement != null && _item.RightDdlStatement is SQLiteCreateTableStatement)
            {
                CopyTable(_leftSchema, _rightSchema,
                    (SQLiteCreateTableStatement)_item.LeftDdlStatement,
                    (SQLiteCreateTableStatement)_item.RightDdlStatement, _leftdb, _rightdb, _leftToRight);
            }
            else if (_item.LeftDdlStatement != null && _item.LeftDdlStatement is SQLiteCreateIndexStatement ||
                _item.RightDdlStatement != null && _item.RightDdlStatement is SQLiteCreateIndexStatement)
            {
                CopyIndex(_leftSchema, _rightSchema,
                    (SQLiteCreateIndexStatement)_item.LeftDdlStatement,
                    (SQLiteCreateIndexStatement)_item.RightDdlStatement, _leftdb, _rightdb, _leftToRight);
            }
            else if (_item.LeftDdlStatement != null && _item.LeftDdlStatement is SQLiteCreateViewStatement ||
                _item.RightDdlStatement != null && _item.RightDdlStatement is SQLiteCreateViewStatement)
            {
                CopyView(_leftSchema, _rightSchema,
                    (SQLiteCreateViewStatement)_item.LeftDdlStatement,
                    (SQLiteCreateViewStatement)_item.RightDdlStatement, _leftdb, _rightdb, _leftToRight);
            }
            else if (_item.LeftDdlStatement != null && _item.LeftDdlStatement is SQLiteCreateTriggerStatement ||
                _item.RightDdlStatement != null && _item.RightDdlStatement is SQLiteCreateTriggerStatement)
            {
                CopyTrigger(_leftSchema, _rightSchema,
                    (SQLiteCreateTriggerStatement)_item.LeftDdlStatement,
                    (SQLiteCreateTriggerStatement)_item.RightDdlStatement, _leftdb, _rightdb, _leftToRight);
            }
            else
                throw new ArgumentException("illegal comparison item provided.");            
        }

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

        private void CopyTrigger(
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> leftSchema,
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> rightSchema, 
            SQLiteCreateTriggerStatement left,
            SQLiteCreateTriggerStatement right, string leftdb, string rightdb, bool leftToRight)
        {
            using (SQLiteConnection leftConn = MakeDbConnection(leftdb))
            {
                leftConn.Open();
                using (SQLiteConnection rightConn = MakeDbConnection(rightdb))
                {
                    rightConn.Open();

                    SQLiteTransaction tx = null;
                    try
                    {
                        string name;
                        if (left != null)
                            name = left.ObjectName.ToString();
                        else
                            name = right.ObjectName.ToString();

                        if (leftToRight)
                        {
                            tx = rightConn.BeginTransaction();
                            ReplaceTrigger(name, rightSchema, rightConn, left, tx, 15, 70);
                        }
                        else
                        {
                            tx = leftConn.BeginTransaction();
                            ReplaceTrigger(name, leftSchema, leftConn, right, tx, 15, 70);
                        }

                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        throw;
                    } // catch
                } // using
            } // using
        }

        private void CopyView(
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> leftSchema,
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> rightSchema, 
            SQLiteCreateViewStatement left,
            SQLiteCreateViewStatement right, string leftdb, string rightdb, bool leftToRight)
        {
            using (SQLiteConnection leftConn = MakeDbConnection(leftdb))
            {
                leftConn.Open();
                using (SQLiteConnection rightConn = MakeDbConnection(rightdb))
                {
                    rightConn.Open();

                    SQLiteTransaction tx = null;
                    try
                    {
                        string name;
                        if (left != null)
                            name = left.ObjectName.ToString();
                        else
                            name = right.ObjectName.ToString();

                        if (leftToRight)
                        {
                            tx = rightConn.BeginTransaction();
                            ReplaceView(name, rightSchema, rightConn, left, tx, 15, 70);
                        }
                        else
                        {
                            tx = leftConn.BeginTransaction();
                            ReplaceView(name, leftSchema, leftConn, right, tx, 15, 70);
                        }

                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        throw;
                    } // catch
                } // using
            } // using
        }

        private void CopyIndex(
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> leftSchema,
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> rightSchema, 
            SQLiteCreateIndexStatement left,
            SQLiteCreateIndexStatement right, string leftdb, string rightdb, bool leftToRight)
        {
            using (SQLiteConnection leftConn = MakeDbConnection(leftdb))
            {
                leftConn.Open();
                using (SQLiteConnection rightConn = MakeDbConnection(rightdb))
                {                    
                    rightConn.Open();

                    SQLiteTransaction tx = null;
                    try
                    {
                        string name;
                        if (left != null)
                            name = left.ObjectName.ToString();
                        else
                            name = right.ObjectName.ToString();

                        if (leftToRight)
                        {
                            tx = rightConn.BeginTransaction();
                            ReplaceIndex(name, rightSchema, rightConn, left, tx, 15, 70);
                        }
                        else
                        {
                            tx = leftConn.BeginTransaction();
                            ReplaceIndex(name, leftSchema, leftConn, right, tx, 15, 70);
                        }

                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        throw;
                    } // catch
                } // using
            } // using
        }

        private void ReplaceView(string name,
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> schema,
            SQLiteConnection conn, SQLiteCreateViewStatement stmt,
            SQLiteTransaction tx, int start, int end)
        {
            if (stmt != null && schema != null)
            {
                // Make sure that all tables that view refers to exist in the target database schema
                // before trying to copy the view there.

                List<string> tables = GetReferredTables(stmt.SelectStatement);
                foreach (string tableName in tables)
                {
                    if (!schema[SchemaObject.Table].ContainsKey(tableName))
                        throw new InvalidOperationException("The view " + stmt.ObjectName.ToString() +
                            " cannot be added because table " + tableName +
                            " does not exist in the target database.\r\nPlease make sure that this table " +
                            " is copied to the target database before trying to copy the view.");
                } // foreach
            }

            NotifyPrimaryProgress(false, start, "deleting view " + name);

            SQLiteCommand cmd = new SQLiteCommand(
                @"DROP VIEW IF EXISTS " + name, conn, tx);
            cmd.ExecuteNonQuery();

            if (_cancelled)
                throw new UserCancellationException();

            if (stmt != null)
            {
                NotifyPrimaryProgress(false, (end - start) / 2 + start, "re-creating view ...");

                // Re-create the view in the right database based on the 
                // view schema in the left database.
                cmd = new SQLiteCommand(stmt.ToString(), conn, tx);
                cmd.ExecuteNonQuery();
            }
        }

        private List<string> GetReferredTables(SQLiteSelectStatement select)
        {
            List<string> res = new List<string>();
            if (select is SQLiteSingleSelectStatement)
            {
                SQLiteSingleSelectStatement singleSelect = (SQLiteSingleSelectStatement)select;
                res.AddRange(GetReferredTablesFromClause(singleSelect.FromClause));
            }
            else if (select is SQLiteMultiSelectStatement)
            {
                SQLiteMultiSelectStatement multi = (SQLiteMultiSelectStatement)select;
                if (multi.First != null)
                    res.AddRange(GetReferredTables(multi.First));
                if (multi.Next != null)
                    res.AddRange(GetReferredTables(multi.Next));
            } // else
            return res;
        }

        private List<string> GetReferredTablesFromClause(SQLiteFromClause from)
        {
            List<string> res = new List<string>();
            foreach (object obj in from.FromTables)
            {
                SQLiteFromTable ft = obj as SQLiteFromTable;
                if (ft != null)
                {
                    if (ft.TableName != null)
                    {
                        string tblName = ft.TableName.ToString().ToLower();
                        if (!res.Contains(tblName))
                            res.Add(tblName);
                    }
                    else if (ft.InternalTable != null)
                    {
                        if (ft.InternalTable.SelectStatement != null)
                            res.AddRange(GetReferredTables(ft.InternalTable.SelectStatement));
                        if (ft.InternalTable.FromClause != null)
                            res.AddRange(GetReferredTablesFromClause(ft.InternalTable.FromClause));
                    } // else
                } // if
            } // foreach
            return res;
        }

        private void ReplaceTrigger(string name,
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> schema,
            SQLiteConnection conn, SQLiteCreateTriggerStatement stmt,
            SQLiteTransaction tx, int start, int end)
        {
            if (stmt != null && schema != null)
            {
                // Make sure that the table that contains the trigger exists in the target database schema
                // before trying to copy the trigger there.

                string tableName = stmt.TableName.ToString().ToLower();
                if (!schema[SchemaObject.Table].ContainsKey(tableName))
                    throw new InvalidOperationException("The trigger " + stmt.ObjectName.ToString() +
                        " cannot be added because table " + tableName +
                        " does not exist in the target database.\r\nCopying the table will automatically" +
                        " add the trigger you are trying to copy.");
            }

            NotifyPrimaryProgress(false, start, "deleting trigger " + name);

            SQLiteCommand cmd = new SQLiteCommand(
                @"DROP TRIGGER IF EXISTS " + name, conn, tx);
            cmd.ExecuteNonQuery();

            if (_cancelled)
                throw new UserCancellationException();

            if (stmt != null)
            {
                NotifyPrimaryProgress(false, (end - start) / 2 + start, "re-creating trigger ...");

                // Re-create the trigger in the right database based on the 
                // trigger schema in the left database.
                cmd = new SQLiteCommand(stmt.ToString(), conn, tx);
                cmd.ExecuteNonQuery();
            }
        }

        private void ReplaceIndex(string name, 
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> schema,
            SQLiteConnection conn, SQLiteCreateIndexStatement stmt, 
            SQLiteTransaction tx, int start, int end)
        {
            if (stmt != null && schema != null)
            {
                // Make sure that the table that contains the index exists in the target database schema
                // before trying to copy the index there.

                string tableName = stmt.OnTable.ToLower();
                if (!schema[SchemaObject.Table].ContainsKey(tableName))
                    throw new InvalidOperationException("The index "+stmt.ObjectName.ToString()+
                        " cannot be added because table " + tableName +
                        " does not exist in the target database.\r\nCopying the table will automatically" +
                        " add the index you are trying to copy.");
            }

            NotifyPrimaryProgress(false, start, "deleting index " + name);

            SQLiteCommand cmd = new SQLiteCommand(
                @"DROP INDEX IF EXISTS " + name, conn, tx);
            cmd.ExecuteNonQuery();

            if (_cancelled)
                throw new UserCancellationException();

            if (stmt != null)
            {
                NotifyPrimaryProgress(false, (end-start)/2+start, "re-creating index ...");

                // Re-create the index in the right database based on the 
                // index schema in the left database.
                cmd = new SQLiteCommand(stmt.ToString(), conn, tx);
                cmd.ExecuteNonQuery();

                if (_cancelled)
                    throw new UserCancellationException();

                NotifyPrimaryProgress(false, end, "re-indexing (may take some time on a big table)");

                // Re-index is necessary after the index was created.
                cmd = new SQLiteCommand(
                    @"REINDEX " + name, conn, tx);
                cmd.ExecuteNonQuery();
            }
        }

        private void ReplaceTable(Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> fromSchema,
               Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> toSchema,
               string name, SQLiteConnection from, SQLiteConnection to, SQLiteCreateTableStatement table)
        {
            long size = 0;
            long count = 0;

            SQLiteCommand cmd = null;
            SQLiteTransaction tx = to.BeginTransaction();

            if (table == null)
            {
                // In this case we need to delete the table in the destination database
                try
                {
                    NotifyPrimaryProgress(false, 50, "Deleting table " + name);
                    cmd = new SQLiteCommand("DROP TABLE " + name, to, tx);
                    cmd.ExecuteNonQuery();

                    tx.Commit();

                    return;
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    throw;
                } // catch
            }

            bool needTemporaryTable = false;
            string tableName = table.ObjectName.ToString();
            string tmpName = Utils.GetTempName(tableName);
            try
            {
                // If the table does not exist in the target database - don't createt a temporary table
                // - instead create the target table immediatly.
                cmd = new SQLiteCommand("SELECT count(*) from sqlite_master where type = 'table' and name = '" +
                    SQLiteParser.Utils.Chop(table.ObjectName.ToString())+"'", to, tx);
                count = (long)cmd.ExecuteScalar();
                if (count > 0)
                {
                    // The table already exists in the target database, so we need to first copy the
                    // source table to a temporary table.
                    NotifyPrimaryProgress(false, 20, "Creating temporary table ..");
                    cmd = new SQLiteCommand(table.ToStatement(tmpName), to, tx);
                    cmd.ExecuteNonQuery();
                    tableName = tmpName;
                    needTemporaryTable = true;
                }
                else
                {
                    // The table does not exist in the target database, so we can copy the source table
                    // directly to the target database.
                    NotifyPrimaryProgress(false, 20, "Creating table ..");
                    cmd = new SQLiteCommand(table.ToString(), to, tx);
                    cmd.ExecuteNonQuery();
                    needTemporaryTable = false;
                }

                NotifyPrimaryProgress(false, 25, "Computing table size ..");
                cmd = new SQLiteCommand("SELECT COUNT(*) FROM " + table.ObjectName.ToString(), from);
                size = (long)cmd.ExecuteScalar();

                if (_cancelled)
                    throw new UserCancellationException();

                tx.Commit();
            }
            catch(Exception ex)
            {
                tx.Rollback();
                throw;
            } // catch

            try
            {
                tx = to.BeginTransaction();

                NotifyPrimaryProgress(false, 30, "Copying table rows ..");
                SQLiteCommand insert = BuildInsertCommand(table, tableName, to, tx);
                cmd = new SQLiteCommand(@"SELECT * FROM " + table.ObjectName, from);

                int prev = 0;
                int curr = 0;
                count = 0;
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        UpdateInsertCommandFields(table, insert, reader);
                        insert.ExecuteNonQuery();

                        count++;
                        if (count % 1000 == 0)
                        {
                            tx.Commit();
                            tx = to.BeginTransaction();

                            curr = (int)(40.0 * count / size + 30);
                            if (curr > prev)
                            {
                                prev = curr;
                                NotifyPrimaryProgress(false, curr, "" + count + " rows copied so far");
                            }
                        } // if

                        if (_cancelled)
                            throw new UserCancellationException();
                    } // while
                } // using

                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();

                if (needTemporaryTable)
                {
                    // Discard the temporary table that was created in the database
                    SQLiteCommand deltemp = new SQLiteCommand(@"DROP TABLE " + tmpName, to);
                    deltemp.ExecuteNonQuery();
                }
                else
                {
                    // Dicard the target table that was created in the database
                    SQLiteCommand deltable = new SQLiteCommand(@"DROP TABLE " + table.ObjectName.ToString(), to);
                    deltable.ExecuteNonQuery();
                } // else

                throw;
            } // catch

            NotifyPrimaryProgress(false, 70, "finalizing table copy operation (may take some time)..");

            // Delete the original table and rename the temporary table to have the same name
            // Note: this step is done at the very end in order to allow the user to cancel the operation
            //       without data loss.
            tx = to.BeginTransaction();
            try
            {
                if (_cancelled)
                    throw new UserCancellationException();

                if (needTemporaryTable)
                {
                    // In case we used a temporary table, we'll now drop the original table
                    // and rename the temporary table to have the name of the original table.

                    SQLiteCommand drop = new SQLiteCommand(
                        @"DROP TABLE " + table.ObjectName.ToString(), to, tx);
                    drop.ExecuteNonQuery();

                    SQLiteCommand alter = new SQLiteCommand(
                        @"ALTER TABLE " + tmpName + " RENAME TO " + table.ObjectName.ToString(), to, tx);
                    alter.ExecuteNonQuery();
                } // if

                // Add all indexes of the replaced table
                int start = 80;
                foreach (SQLiteDdlStatement stmt in fromSchema[SchemaObject.Index].Values)
                {
                    if (_cancelled)
                        throw new UserCancellationException();

                    SQLiteCreateIndexStatement cindex = stmt as SQLiteCreateIndexStatement;
                    if (SQLiteParser.Utils.Chop(cindex.OnTable).ToLower() ==
                        SQLiteParser.Utils.Chop(table.ObjectName.ToString()).ToLower())
                    {
                        ReplaceIndex(cindex.ObjectName.ToString(), null, to, cindex, tx, start, start + 1);
                        start++;
                        if (start == 90)
                            start = 89;
                    }
                } // foreach

                // Add all table triggers of the replaced table
                start = 90;
                foreach (SQLiteDdlStatement stmt in fromSchema[SchemaObject.Trigger].Values)
                {
                    SQLiteCreateTriggerStatement trigger = stmt as SQLiteCreateTriggerStatement;
                    if (SQLiteParser.Utils.Chop(trigger.TableName.ToString()).ToLower() ==
                        SQLiteParser.Utils.Chop(table.ObjectName.ToString()).ToLower())
                    {
                        ReplaceTrigger(trigger.ObjectName.ToString(), null, to, trigger, tx, start, start + 1);
                        start++;
                        if (start == 100)
                            start = 99;
                    }
                } // foreach

                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();

                if (needTemporaryTable)
                {
                    // Discard the temporary table that was created in the database
                    SQLiteCommand deltemp = new SQLiteCommand(@"DROP TABLE " + tmpName, to);
                    deltemp.ExecuteNonQuery();
                }
                else
                {
                    // Dicard the target table that was created in the database
                    SQLiteCommand deltable = new SQLiteCommand(@"DROP TABLE " + table.ObjectName.ToString(), to);
                    deltable.ExecuteNonQuery();
                } // else

                throw;
            } // catch

            NotifyPrimaryProgress(false, 99, "total of " + count + " rows copied");
        }

        private void UpdateInsertCommandFields(SQLiteCreateTableStatement table, 
            SQLiteCommand insert, SQLiteDataReader reader)
        {
            for (int i = 0; i < table.Columns.Count; i++)
            {
                SQLiteColumnStatement col = table.Columns[i];
                string prmName = Utils.GetColumnParameterName(col.ObjectName.ToString());
                string colName = SQLiteParser.Utils.Chop(col.ObjectName.ToString());
                insert.Parameters[prmName].Value = reader[colName];
            } // for
        }

        private SQLiteCommand BuildInsertCommand(SQLiteCreateTableStatement table, string tableName, 
            SQLiteConnection conn, SQLiteTransaction tx)
        {
            SQLiteCommand res = new SQLiteCommand();

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO " + tableName + " (");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                SQLiteColumnStatement col = table.Columns[i];
                sb.Append(col.ObjectName.ToString());
                if (i < table.Columns.Count - 1)
                    sb.Append(",");
            } // for
            sb.Append(") VALUES (");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                SQLiteColumnStatement col = table.Columns[i];
                string prmName = Utils.GetColumnParameterName(col.ObjectName.ToString());
                sb.Append(prmName);
                if (i < table.Columns.Count - 1)
                    sb.Append(",");

                res.Parameters.Add(prmName, Utils.GetDbType(col.ColumnType));
            } // for
            sb.Append(")");

            res.CommandText = sb.ToString();
            res.Connection = conn;
            res.Transaction = tx;

            return res;
        }

        private SQLiteConnection MakeDbConnection(string fpath)
        {
            SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder();
            sb.DataSource = fpath;
            return new SQLiteConnection(sb.ConnectionString);
        }

        private void CopyTable(
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> leftSchema,
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> rightSchema, 
            SQLiteCreateTableStatement left,
            SQLiteCreateTableStatement right, 
            string leftdb, 
            string rightdb, 
            bool leftToRight)
        {
            using (SQLiteConnection leftConn = MakeDbConnection(leftdb))
            {
                leftConn.Open();
                using (SQLiteConnection rightConn = MakeDbConnection(rightdb))
                {
                    rightConn.Open();

                    string name;
                    if (left != null)
                        name = left.ObjectName.ToString();
                    else
                        name = right.ObjectName.ToString();

                    if (leftToRight)
                        ReplaceTable(leftSchema, rightSchema, name, leftConn, rightConn, left);
                    else
                        ReplaceTable(rightSchema, leftSchema, name, rightConn, leftConn, right);
                } // using
            } // using            
        }
        #endregion

        #region Private Variables
        private ProgressEventArgs _pevent;
        private Thread _worker;
        private string _name;
        private bool _cancelled;
        private Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> _leftSchema;
        private Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> _rightSchema;
        private SchemaComparisonItem _item;
        private string _leftdb;
        private string _rightdb;
        private bool _leftToRight;
        private static ILog _log = LogManager.GetLogger(typeof(ItemCopier));
        #endregion
    }
}
