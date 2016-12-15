using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.ComponentModel;
using System.Data.SQLite;
using SQLiteParser;
using log4net;
using Common;

namespace SQLiteTurbo
{
    /// <summary>
    /// This class provides all the methods that are necessary to compare
    /// two SQLite files. It includes support for both schema and data 
    /// comparisons.
    /// </summary>
    public class CompareWorker : IWorker
    {
        #region Events
        /// <summary>
        /// Fired whenever some progress is made in the comparison process
        /// </summary>
        public event EventHandler<ProgressEventArgs> ProgressChanged;
        #endregion

        #region Constructors
        public CompareWorker(CompareParams cp)
        {
            _params = cp;

            // Initialize the lexical scanner and the parser objects that will be used
            // to analyze all SQLite schema objects.
            _parser.scanner = _scanner;

            // Initialize the notification object
            _pevent = new ProgressEventArgs(false, 0, null, null);
            _pevent.NestedProgress = new ProgressEventArgs(false, 0, null, null);
        }
        #endregion

        #region IWorker Implementation
        /// <summary>
        /// Begin the comparison process.
        /// </summary>
        public void BeginWork()
        {
            if (_worker != null && _worker.IsAlive)
                throw new ApplicationException("comparison is already in progress");

            _result = null;
            _cancelled = false;
            ThreadStart ts = delegate
            {
                try
                {
                    NotifyPrimaryProgress(false, 0, "Parsing DB schema of " + _params.LeftDbPath + "...");
                    Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> left = ParseDB(_params.LeftDbPath);
                    _leftSchema = left;

                    NotifyPrimaryProgress(false, 10, "Parsing DB schema of " + _params.RightDbPath + "...");
                    Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> right = ParseDB(_params.RightDbPath);
                    _rightSchema = right;

                    NotifyPrimaryProgress(false, 20, "Comparing schema information ...");
                    _result = CompareSchema(left, right);

                    if (_params.ComparisonType == ComparisonType.CompareSchemaAndData)
                    {
                        NotifyPrimaryProgress(false, 50, "Comparing table data ...");
                        CompareTables(_params.LeftDbPath, _params.RightDbPath, _result, _params.IsCompareBlobFields);
                    }

                    NotifyPrimaryProgress(true, 100, "Finished comparison");
                }
                catch (UserCancellationException cex)
                {
                    _log.Debug("The user chose to cancel a compare operation");
                    if (_result != null)
                        CleanupTempFiles(_result);
                    NotifyPrimaryProgress(true, 100, cex);
                }
                catch (Exception ex)
                {
                    _log.Error("failed to compare databases", ex);
                    if (_result != null)
                        CleanupTempFiles(_result);
                    NotifyPrimaryProgress(true, 100, ex);
                } // catch
            };
            _worker = new Thread(ts);
            _worker.IsBackground = true;
            _worker.Name = "CompareWorker thread";

            _worker.Start();
        }

        /// <summary>
        /// Cancel the comparison process
        /// </summary>
        public void Cancel()
        {
            _log.Debug("CompareWorker.Cancel called");
            _cancelled = true;
            IWorker comparer = _tableComparer;
            if (comparer != null)
                comparer.Cancel();
        }

        /// <summary>
        /// Return the result of the operation
        /// </summary>
        public object Result
        {
            get { return _result; }
        }

        /// <summary>
        /// This worker supports dual progress notifications
        /// </summary>
        public bool SupportsDualProgress
        {
            get { return true; }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Returns the left schema dictionary
        /// </summary>
        /// <remarks>Call only after the worker has finished doing its job</remarks>
        public Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> LeftSchema
        {
            get { return _leftSchema; }
        }

        /// <summary>
        /// Returns the right schema dictionary
        /// </summary>
        /// <remarks>Call only after the worker has finished doing its job</remarks>
        public Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> RightSchema
        {
            get { return _rightSchema; }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// For every table that has the same schema in both databases - compare its
        /// data and update the _result object if necessary.
        /// </summary>
        /// <param name="leftdb">The leftdb.</param>
        /// <param name="rightdb">The rightdb.</param>
        /// <param name="changes">The list of schema changes to check</param>
        private void CompareTables(
            string leftdb, string rightdb,
            Dictionary<SchemaObject, List<SchemaComparisonItem>> changes, bool allowBlobComparison)
        {
            // Go over all tables and select for comparison only those tables that have identical
            // schema.
            List<SchemaComparisonItem> clist = changes[SchemaObject.Table];
            int total = clist.Count;
            int offset = 0;
            int prev = -1;

            foreach (SchemaComparisonItem item in clist)
            {
                if (item.Result == ComparisonResult.Same || item.Result == ComparisonResult.DifferentSchema)
                {
                    offset++;
                    double progress = 50.0 + 50.0 * offset / total;
                    if ((int)progress > prev)
                    {
                        prev = (int)progress;
                        NotifyPrimaryProgress(false, prev, "Comparing data for table [" + item.LeftDdlStatement.ObjectName.ToString() + "]..");
                    }

                    IWorker tableComparer =
                        new TableCompareWorker((SQLiteCreateTableStatement)item.LeftDdlStatement,
                        (SQLiteCreateTableStatement)item.RightDdlStatement, leftdb, rightdb, allowBlobComparison);

                    _tableComparer = new SyncWorker(tableComparer);
                    EventHandler<ProgressEventArgs> eh = new EventHandler<ProgressEventArgs>(delegate(object s, ProgressEventArgs e)
                    {
                        NotifySecondaryProgress(e.IsDone, e.Progress, e.Message);
                    });

                    try
                    {
                        _tableComparer.ProgressChanged += eh;

                        _tableComparer.BeginWork();

                        TableChanges tableChanges = (TableChanges)_tableComparer.Result;
                        item.TableChanges = tableChanges;
                    }
                    catch (UserCancellationException uce)
                    {
                        // Ignore
                    }
                    catch (Exception ex)
                    {
                        // The tables data cannot be compared so ignore.
                        item.ErrorMessage = ex.Message;
                    }
                    finally
                    {
                        _tableComparer.ProgressChanged -= eh;
                        _tableComparer = null;
                    }

                    if (_cancelled)
                        throw new UserCancellationException();
                }
            } // foreach
        }

        /// <summary>
        /// Cleanup any table changes leftovers
        /// </summary>
        /// <param name="result">The global comparison results object</param>
        private void CleanupTempFiles(Dictionary<SchemaObject, List<SchemaComparisonItem>> result)
        {
            List<SchemaComparisonItem> tableItems = result[SchemaObject.Table];
            foreach (SchemaComparisonItem item in tableItems)
            {
                if (item.TableChanges != null)
                    item.TableChanges.Dispose();
            } // foreach
        }

        /// <summary>
        /// Check if the specified talbe has primary key(s)
        /// </summary>
        /// <param name="table">the table to check</param>
        /// <returns>TRUE if the table has primary key(s)</returns>
        private bool HasPrimaryKeys(SQLiteCreateTableStatement table)
        {
            List<SQLiteColumnStatement> res = Utils.GetPrimaryColumns(table);
            if (res.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Compare the schema information in the specified left/right database schema objects
        /// and return a list of differences.
        /// </summary>
        /// <param name="left">The schema information for the left database.</param>
        /// <param name="right">The schema information for the right database.</param>
        /// <returns>A dictionary that maps, for every schema object type - a list of differences
        /// that were found between the two databases.</returns>
        private Dictionary<SchemaObject, List<SchemaComparisonItem>> CompareSchema(
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> left, 
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> right)
        {
            Dictionary<SchemaObject, List<SchemaComparisonItem>> res = 
                new Dictionary<SchemaObject, List<SchemaComparisonItem>>();
            res.Add(SchemaObject.Table, new List<SchemaComparisonItem>());
            res.Add(SchemaObject.Index, new List<SchemaComparisonItem>());
            res.Add(SchemaObject.Trigger, new List<SchemaComparisonItem>());
            res.Add(SchemaObject.View, new List<SchemaComparisonItem>());

            // Prepare auxiliary variables used for notifying progress
            int total = left[SchemaObject.Table].Count + left[SchemaObject.Index].Count +
                left[SchemaObject.Trigger].Count + left[SchemaObject.View].Count +
                right[SchemaObject.Table].Count + right[SchemaObject.Index].Count +
                right[SchemaObject.Trigger].Count + right[SchemaObject.View].Count;

            // First locate all objects that exist in the left DB but not in the right DB
            int index = 0;
            foreach (SchemaObject so in left.Keys)
            {
                foreach (string objname in left[so].Keys)
                {
                    // Ignore internal sqlite tables
                    if (objname.StartsWith("sqlite_"))
                        continue;

                    if (!right[so].ContainsKey(objname))
                    {
                        // This object exists only in the left DB
                        SchemaComparisonItem item = 
                            new SchemaComparisonItem(objname, left[so][objname], null, ComparisonResult.ExistsInLeftDB);
                        res[so].Add(item);
                    }
                    else if (!left[so][objname].Equals(right[so][objname]))
                    {
                        // This object exists in both the left DB and the right DB, but it has
                        // different schema layout.
                        SchemaComparisonItem item =
                            new SchemaComparisonItem(objname, left[so][objname], right[so][objname], ComparisonResult.DifferentSchema);
                        res[so].Add(item);
                    }
                    else
                    {
                        // This object exists in both the left DB abd the right DB and it has
                        // the same schema layout in both databases.
                        SchemaComparisonItem item =
                            new SchemaComparisonItem(objname, left[so][objname], right[so][objname], ComparisonResult.Same);
                        res[so].Add(item);
                    }

                    double progress = 100.0 * index++ / total;
                    NotifySecondaryProgress(false, (int)progress, "Compared object " + objname);

                    if (_cancelled)
                        throw new UserCancellationException();
                } // foreach
            } // foreach

            // Next locate all objects that exist only in the right DB
            foreach (SchemaObject so in right.Keys)
            {
                foreach (string objname in right[so].Keys)
                {
                    // Ignore internal sqlite tables
                    if (objname.StartsWith("sqlite_"))
                        continue;

                    if (!left[so].ContainsKey(objname))
                    {
                        // This object exists only in the right DB
                        SchemaComparisonItem item =
                            new SchemaComparisonItem(objname, null, right[so][objname], ComparisonResult.ExistsInRightDB);
                        res[so].Add(item);
                    } // if

                    double progress = 100.0 * index++ / total;
                    NotifySecondaryProgress(false, (int)progress, "Compared object " + objname);

                    if (_cancelled)
                        throw new UserCancellationException();
                } // foreach
            } // foreach

            NotifySecondaryProgress(true, 100, "Finished schema comparisons");

            return res;
        }

        /// <summary>
        /// Parse the schema information in the specified DB file and return the information
        /// in a form of a dictionary that provides, for every type of database object (table, index,
        /// trigger and view) - a dictionary of DDL statements that are keyed by the names of these
        /// objects.
        /// </summary>
        /// <param name="fpath">The path to the SQLite database file</param>
        /// <returns>The schema information for the specified file.</returns>
        private Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> ParseDB(string fpath)
        {
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> res =
                new Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>>();
            res.Add(SchemaObject.Table, new Dictionary<string,SQLiteDdlStatement>());
            res.Add(SchemaObject.View, new Dictionary<string,SQLiteDdlStatement>());
            res.Add(SchemaObject.Trigger, new Dictionary<string,SQLiteDdlStatement>());
            res.Add(SchemaObject.Index, new Dictionary<string,SQLiteDdlStatement>());

            SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder();
            sb.DataSource = fpath;
            sb.ReadOnly = true;

            using (SQLiteConnection conn = new SQLiteConnection(sb.ConnectionString))
            {
                conn.Open();

                SQLiteCommand queryCount = new SQLiteCommand(
                    @"SELECT COUNT(*) FROM sqlite_master WHERE sql IS NOT NULL", conn);
                long count = (long)queryCount.ExecuteScalar();

                int index = 0;
                SQLiteCommand query = new SQLiteCommand(
                    @"SELECT * FROM sqlite_master WHERE sql IS NOT NULL", conn);
                using (SQLiteDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string sql = (string)reader["sql"];
                        sql = Utils.StripComments(sql);
                        _scanner.SetSource(sql, 0);

                        // Request the parser to parse the SQL statement
                        bool ok = _parser.Parse();
                        if (!ok)
                            throw new ApplicationException("invalid sql string");

                        SQLiteDdlStatement stmt = SQLiteDdlMain.GetStatement();
                        if (stmt is SQLiteCreateTableStatement)
                            res[SchemaObject.Table].Add(SQLiteParser.Utils.Chop(stmt.ObjectName.ToString()).ToLower(), stmt);
                        else if (stmt is SQLiteCreateIndexStatement)
                            res[SchemaObject.Index].Add(SQLiteParser.Utils.Chop(stmt.ObjectName.ToString()).ToLower(), stmt);
                        else if (stmt is SQLiteCreateViewStatement)
                            res[SchemaObject.View].Add(SQLiteParser.Utils.Chop(stmt.ObjectName.ToString()).ToLower(), stmt);
                        else if (stmt is SQLiteCreateTriggerStatement)
                            res[SchemaObject.Trigger].Add(SQLiteParser.Utils.Chop(stmt.ObjectName.ToString()).ToLower(), stmt);
                        else
                            throw new ApplicationException("illegal ddl statement type [" + stmt.GetType().FullName + "]");

                        double progress = (100.0 * index++) / count;
                        NotifySecondaryProgress(false, (int)progress, "Parsed object " + stmt.ObjectName.ToString());

                        if (_cancelled)
                            throw new UserCancellationException();
                    } // while
                } // using
            } // using

            NotifySecondaryProgress(true, 100, "Finished parsing DB " + fpath);

            return res;
        }

        private void NotifyPrimaryProgress(bool done, int progress, string msg)
        {
            _pevent.IsDone = done;
            _pevent.Progress = progress;
            _pevent.Message = msg;
            _pevent.Error = null;

            _log.Debug("CompareWorker.NotifyPrimaryProgress(" + done + "," + progress + "," + msg + ")");
            if (ProgressChanged != null)
                ProgressChanged(this, _pevent);
        }

        private void NotifyPrimaryProgress(bool done, int progress, Exception error)
        {
            _pevent.IsDone = done;
            _pevent.Progress = progress;
            _pevent.Message = null;
            _pevent.Error = error;

            _log.Debug("CompareWorker.NotifyPrimaryProgress(" + done + "," + progress + ",E:"+error.Message + ")");
            if (ProgressChanged != null)
                ProgressChanged(this, _pevent);
        }

        private void NotifySecondaryProgress(bool done, int progress, string msg)
        {
            _pevent.NestedProgress.IsDone = done;
            _pevent.NestedProgress.Progress = progress;
            _pevent.NestedProgress.Message = msg;
            _pevent.NestedProgress.Error = null;

            if (ProgressChanged != null)
                ProgressChanged(this, _pevent);
        }

        private void NotifySecondaryProgress(bool done, int progress, Exception error)
        {
            _pevent.NestedProgress.IsDone = done;
            _pevent.NestedProgress.Progress = progress;
            _pevent.NestedProgress.Message = null;
            _pevent.NestedProgress.Error = error;

            if (ProgressChanged != null)
                ProgressChanged(this, _pevent);
        }
        #endregion

        #region Private Variables
        private ProgressEventArgs _pevent;
        private CompareParams _params;
        private bool _cancelled;
        private Thread _worker;
        private IWorker _tableComparer = null;
        private Scanner _scanner = new Scanner();
        private Parser _parser = new Parser();
        private Dictionary<SchemaObject, List<SchemaComparisonItem>> _result;
        private Dictionary<string, TableChanges> _tchanges;
        private Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> _leftSchema;
        private Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> _rightSchema;
        private ILog _log = LogManager.GetLogger(typeof(CompareWorker));
        #endregion
    }

}
