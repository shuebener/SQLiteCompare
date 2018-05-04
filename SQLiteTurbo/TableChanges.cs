using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Data;
using System.Data.SQLite;
using System.Threading;
using SQLiteParser;
using log4net;
using Common;

namespace SQLiteTurbo
{
    /// <summary>
    /// This class is responsible to provide the list of table row differences that were
    /// computed by the table compare worker object. In order to prevent memory problems 
    /// when comparing very large tables, it uses a temporary database file (called the
    /// differences database file) in order to store all differences that were found 
    /// during the comparison process. In addition - it provides an easy interface for
    /// accessing these differences quickly.
    /// </summary>
    public class TableChanges : IDisposable
    {
		#region Public Constants
		/// <summary>
		/// This is the name, in the difference database, of the table that contains
		/// all the differences that refer to rows that exist only in the left database.
		/// </summary>
		public const string EXISTS_IN_LEFT_OR_RIGHT_TABLE_NAME = "exists_in_left_or_right";
		/// <summary>
		/// This is the name, in the difference database, of the table that contains
		/// all the differences that refer to rows that exist only in the left database.
		/// </summary>
		public const string EXISTS_IN_LEFT_TABLE_NAME = "exists_in_left";

        /// <summary>
        /// This is the name, in the differences database, of the table that contains
        /// all the differences that refer to rows that exist only in the right database.
        /// </summary>
        public const string EXISTS_IN_RIGHT_TABLE_NAME = "exists_in_right";

        /// <summary>
        /// This is the name, in the differences database, of the table that contains
        /// all the differences that refer to rows that exist in both left and right
        /// databases but have different column values there.
        /// </summary>
        public const string DIFFERENT_ROWS_TABLE_NAME = "different_rows";

        /// <summary>
        /// This is the name, in the differences database, of the table that contains
        /// all the differences that refer to rows that exist in both left and right
        /// databases and have the same column values.
        /// </summary>
        public const string SAME_ROWS_TABLE_NAME = "same_rows";

        /// <summary>
        /// When working in update mode (after BeginUpdate was called but before EndUpdate
        /// was called), this constant determines how many changes can be added to the 
        /// various difference tables in the temporary differences database before
        /// automatically commiting the changes to the differences database.
        /// </summary>
        public const int MAX_PENDING_CHANGES = 1000;
        #endregion

        #region Constructors
        /// <summary>
        /// This static constructor is used to automatically delete any left-over change files
        /// from the temporary folder.
        /// </summary>
        static TableChanges()
        {
            // Create a mutex for this application instance
            _appmutex = new Mutex(false, "Global\\" + Utils.NormalizeName(_appid.ToString()));
        }

        /// <summary>
        /// Constructs a new table-changes object. Under the cover - this object creates
        /// a temporary database file that will be used to store all table differences.
        /// </summary>
        /// <param name="leftTable">The left table.</param>
        /// <param name="rightTable">The right table.</param>
        /// <param name="leftdb">The path to the left database file</param>
        /// <param name="rightdb">The path to the right database file</param>
        public TableChanges(SQLiteCreateTableStatement leftTable, SQLiteCreateTableStatement rightTable, 
            string leftdb, string rightdb)
        {
            _leftTable = leftTable;
            _rightTable = rightTable;

            if (leftTable != null)
                _tableName = leftTable.ObjectName;
            else
                _tableName = rightTable.ObjectName;
            _leftdb = leftdb;
            _rightdb = rightdb;

            // Decide if we'll create an actual difference file or work with an in-memory difference
            // database based on the maximum number of rows in both databases.
            long leftCount = GetTableRowsCount(_leftdb, _tableName.ToString());
            long rightCount = GetTableRowsCount(_rightdb, _tableName.ToString());
            if (leftCount + rightCount < CACHE_BUFFER_SIZE)
            {
                // We'll use an in-memory database for storing the differences
                SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder();
                sb.DataSource = ":memory:";
                sb.PageSize = 4096;
                _main = new SQLiteConnection(sb.ConnectionString);
                _main.Open();

                // Create the differences DB schema
                CreateDifferenceDbSchema(_main);

                // Mark that the difference database is stored in-memory
                _inmemory = true;
            }
            else
            {
                // Create a new temporary database for storing the specified table's differences
                _main = CreateTableChangesDB(_tableName.ToString());

                // Mark that the differences database is stored in a temporary file.
                _inmemory = false;
            }

            // Create the commands that are necessary for adding table changes to the differences database.
            _addDifferent = CreateAddCommand(DIFFERENT_ROWS_TABLE_NAME);
            _addExistsInLeft = CreateAddCommand(EXISTS_IN_LEFT_TABLE_NAME);
            _addExistsInRight = CreateAddCommand(EXISTS_IN_RIGHT_TABLE_NAME);
            _addSame = CreateAddCommand(SAME_ROWS_TABLE_NAME);
			_addBoth = CreateAddCommand(EXISTS_IN_LEFT_OR_RIGHT_TABLE_NAME);

            // Add to the active change files
            lock (_activeChanges)
                _activeChanges.Add(this);
        }

        /// <summary>
        /// Dispose from internal resources.
        /// </summary>
        ~TableChanges()
        {
			Dispose(false);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Get the schema object for the left table that was used in the comparison
        /// </summary>
        public SQLiteCreateTableStatement LeftTable
        {
            get { return _leftTable; }
        }

        /// <summary>
        /// Get the schema object for the right table that was used in the comparison
        /// </summary>
        public SQLiteCreateTableStatement RightTable
        {
            get { return _rightTable; }
        }

        /// <summary>
        /// In case the object contains precise results - TRUE. Otherwise - FALSE>
        /// </summary>
        /// <remarks>The table changes object can contain unprecise results if the user did rows copying
        /// or changed fields in rows manually.</remarks>
        public bool HasPreciseResults
        {
            get { return _precise; }
            set { _precise = value; }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Export the entire changes list to CSV file format
        /// </summary>
        /// <param name="fpath">The path to the CSV file</param>
        /// <param name="exportUpdates">if set to <c>true</c> [export updates].</param>
        /// <param name="exportAdded">if set to <c>true</c> [export added].</param>
        /// <param name="exportDeleted">if set to <c>true</c> [export deleted].</param>
        /// <param name="handler">The handler.</param>
        public void ExportToCSV(string fpath, bool exportUpdates, bool exportAdded, bool exportDeleted, ExportProgressHandler handler)
        {
            if (File.Exists(fpath))
                File.Delete(fpath);

            using (StreamWriter writer = new StreamWriter(fpath, false, Encoding.Unicode))
            {
                // Prepare list of difference types to write
                List<string> dtypes = new List<string>();
                if (exportUpdates)
                    dtypes.Add(TableChanges.DIFFERENT_ROWS_TABLE_NAME);
                if (exportAdded)
                    dtypes.Add(TableChanges.EXISTS_IN_RIGHT_TABLE_NAME);
                if (exportDeleted)
                    dtypes.Add(TableChanges.EXISTS_IN_LEFT_TABLE_NAME);

                // This variable will hold the number of rows that were already exported
                long exported = 0;

                // This variable holds the total number of changes to write
                long total = GetTotalChangesCount(dtypes.ToArray());

                writer.WriteLine();
                writer.WriteLine("Changes in table " + _tableName.ToString());
                writer.WriteLine();

                writer.Write("CHANGE TYPE,");

                // Compute list of common columns to both database tables
                List<string> common = GetCommonColumnNames();

                // Write them out
                for (int i = 0; i < common.Count; i++)
                {
                    writer.Write(EscapeColumn(common[i]));
                    if (i < common.Count - 1)
                        writer.Write(",");
                } // for
                writer.WriteLine();

                // Write all changes to the CSV file
                foreach (string dt in dtypes)
                    ExportChanges(writer, common, dt, ref exported, total, handler);
            } // using
        }

        /// <summary>
        /// Export the entire list of table changes (for multiple tables) to CSV file format
        /// </summary>
        /// <param name="fpath">File to write to</param>
        /// <param name="list">List of table changes</param>
        /// <param name="fpath">The path to the CSV file</param>
        /// <param name="exportUpdates">if set to <c>true</c> [export updates].</param>
        /// <param name="exportAdded">if set to <c>true</c> [export added].</param>
        /// <param name="exportDeleted">if set to <c>true</c> [export deleted].</param>
        /// <param name="handler">The handler.</param>
        public static void ExportMultipleToCSV(string fpath, List<TableChanges> list, bool exportUpdates, 
            bool exportAdded, bool exportDeleted, ExportProgressHandler handler)
        {
            if (File.Exists(fpath))
                File.Delete(fpath);

            using (StreamWriter writer = new StreamWriter(fpath, false, Encoding.Unicode))
            {
                // Prepare list of difference types to write
                List<string> dtypes = new List<string>();
                if (exportUpdates)
                    dtypes.Add(TableChanges.DIFFERENT_ROWS_TABLE_NAME);
                if (exportAdded)
                    dtypes.Add(TableChanges.EXISTS_IN_RIGHT_TABLE_NAME);
                if (exportDeleted)
                    dtypes.Add(TableChanges.EXISTS_IN_LEFT_TABLE_NAME);

                // This variable will hold the number of rows that were already exported
                long exported = 0;

                // This variable holds the total number of changes to write
                long total = 0;
                foreach (TableChanges tc in list)
                    total += tc.GetTotalChangesCount(dtypes.ToArray());

                foreach (TableChanges tc in list)
                {
                    if (tc.GetTotalChangesCount(dtypes.ToArray()) == 0)
                        continue;

                    writer.WriteLine();
                    writer.WriteLine("Changes in table "+tc._tableName.ToString());
                    writer.WriteLine();

                    writer.Write("CHANGE TYPE,");

                    // Compute list of common columns to both database tables
                    List<string> common = tc.GetCommonColumnNames();

                    // Write them out
                    for (int i = 0; i < common.Count; i++)
                    {
                        writer.Write(EscapeColumn(common[i]));
                        if (i < common.Count - 1)
                            writer.Write(",");
                    } // for
                    writer.WriteLine();

                    foreach (string dt in dtypes)
                    {
                        tc.ExportChanges(writer, common, dt, ref exported, total, handler);
                    }
                } // foreach
            } // using
        }

        /// <summary>
        /// Remove all active change files (done before application exits).
        /// </summary>
        public static void RemoveActiveChangeFiles()
        {
            lock (_activeChanges)
            {
                foreach (TableChanges tc in _activeChanges)
                {
                    IDisposable d = (IDisposable)tc;
                    d.Dispose();
                } // foreach
                _activeChanges.Clear();
            }
        }

        /// <summary>
        /// This method is called when the application starts in order to remove stale change files
        /// from the temporary folder
        /// </summary>
        public static void RemoveStaleChangeFiles()
        {
            Dictionary<string, string> mutexes = new Dictionary<string, string>();
            Dictionary<string, string> removed = new Dictionary<string, string>();

            string folder = GetTempFolder();
            string[] files = Directory.GetFiles(folder);
            foreach (string fpath in files)
            {
                Match m = _tchangeRx.Match(fpath);
                if (m.Success)
                {
                    if (mutexes.ContainsKey(m.Groups[1].Value))
                        continue;

                    try
                    {
                        string mname = "Global\\" + m.Groups[1].Value;

                        if (!removed.ContainsKey(m.Groups[1].Value))
                        {
                            Mutex mutex = Mutex.OpenExisting(mname);
                            mutexes.Add(m.Groups[1].Value, null);
                            continue;
                        }
                    }
                    catch (WaitHandleCannotBeOpenedException ex)
                    {
                        removed.Add(m.Groups[1].Value, null);
                    } // catch

                    if (removed.ContainsKey(m.Groups[1].Value))
                    {
                        try
                        {
                            // The application that created this file was closed
                            // so I can delete the file.
                            File.Delete(fpath);
                        }
                        catch (Exception io)
                        {
                            // TODO: log
                        }
                    } // if
                } // if
            } // foreach
        }

        /// <summary>
        /// Returns the total number of changes for the specified difference types.
        /// </summary>
        /// <param name="diffTypes">A set of difference types to check</param>
        /// <returns>The total number of changes found for the specified difference types</returns>
        public long GetTotalChangesCount(string[] diffTypes)
        {
            long res = 0;

            foreach (string diffname in diffTypes)
            {
                SQLiteCommand query = new SQLiteCommand(
                    @"select count(*) from " + diffname, _main);
                res += (long)query.ExecuteScalar();
            }

            return res;
        }

        /// <summary>
        /// Returns the total number of changes for all difference categories
        /// </summary>
        /// <returns></returns>
        public long GetTotalChangesCount()
        {
            return GetTotalChangesCount(new string[] 
                { EXISTS_IN_LEFT_TABLE_NAME, 
                  EXISTS_IN_RIGHT_TABLE_NAME, DIFFERENT_ROWS_TABLE_NAME });
        }

        /// <summary>
        /// Returns a single change item for the specified row Id and difference type
        /// </summary>
        /// <remarks>This method employes a caching mechanism in order to make future
        /// requests near the requested row id much faster</remarks>
        /// <param name="diffType">The difference type to fetch</param>
        /// <param name="rowId">The index of the row to fetch</param>
        /// <returns>The requested change item.</returns>
        public TableChangeItem GetChangeItem(string diffType, long rowIndex)
        {
            // Check if the requested row id exists in the cache
            if (_cachediff == diffType && rowIndex - _cacheOffset < _cache.Count && rowIndex -_cacheOffset >= 0 && _cache.Count > 0)
                return _cache[(int)(rowIndex - _cacheOffset)];

            // The requested row id does not exist in the cache so do 2 things:
            // 1. Clear the current difference cache
            // 2. Populate it again from the database.

            _cachediff = diffType;
            _cacheOffset = rowIndex - CACHE_BUFFER_SIZE/2;
            if (_cacheOffset < 0)
                _cacheOffset = 0;
            _cache.Clear();
            _cache = GetChanges(diffType, CACHE_BUFFER_SIZE, _cacheOffset);
            
            // Return the cached copy
            if (rowIndex - _cacheOffset < _cache.Count && rowIndex - _cacheOffset >= 0 && _cache.Count > 0)
                return _cache[(int)(rowIndex - _cacheOffset)];

            throw new IndexOutOfRangeException();
        }

        /// <summary>
        /// Returns an array of change items for all rows with any of the difference types that
        /// were provided.
        /// </summary>
        /// <param name="limit">A maximum limit to the number of changes to return</param>
        /// <param name="offset">The offset to the differences items</param>
        /// <returns>A list of table change items</returns>
        public List<TableChangeItem> GetChanges(string diffType, int limit, long offset)
        {
            // Query all rows from the different database according to the search criteria
            SQLiteCommand query = BuildSelectChangesCommand(diffType, limit, offset, _main);

            using (SQLiteDataReader reader = query.ExecuteReader())
            {
                if (!reader.HasRows)
                    return new List<TableChangeItem>();

                // Open connection to the left database
                using (SQLiteConnection left = MakeReadOnlyConnection(_leftdb))
                {
                    left.Open();

                    // Open connection to the right database
                    using (SQLiteConnection right = MakeReadOnlyConnection(_rightdb))
                    {
                        right.Open();
                        
                        // Prepare command for retrieving the left database table row and the
                        // right database table row.
                        SQLiteCommand readLeftRow = PrepareReadRowCommand(_leftTable, left);
                        SQLiteCommand readRightRow = PrepareReadRowCommand(_rightTable, right);

                        List<TableChangeItem> res = new List<TableChangeItem>();
                        while (reader.Read())
                        {
                            long itemRowId = (long)reader["RowID"];
                            ComparisonResult cres = (ComparisonResult)(long)reader["ComparisonResult"];
                            long leftRowId = (long)reader["LeftRowID"];
                            long rightRowId = (long)reader["RightRowID"];

                            // In case the comparison included the list of changed BLOB fields - 
                            // parse their names from the change item and store into the item in memory.
                            List<string> changedBlobs = null;
                            object blobs = reader["ChangedBlobs"];
                            if (blobs != DBNull.Value)
                                changedBlobs = Deserialize((string)reader["ChangedBlobs"]);
                            
                            TableChangeItem item = 
                                BuildChangeItem(cres, readLeftRow, leftRowId, readRightRow, rightRowId);
                            item.ChangeItemRowId = itemRowId;
                            item.ChangedBlobsColumnNames = changedBlobs;

                            res.Add(item);
                        } // while

                        return res;
                    } // using
                } // using
            } // using
        }

        /// <summary>
        /// Sets the field as specified by the difference type and difference item row id and column name in the
        /// right or left table with the updated value.
        /// </summary>
        /// <param name="diff">The difference table that contains the change item</param>
        /// <param name="itemRowId">The change item row index</param>
        /// <param name="columnName">The name of the column whose corresponding field is updated</param>
        /// <param name="right">TRUE means that the update should be performed on the right table, FALSE means
        /// that the update should be performed on the left table.</param>
        /// <param name="updatedValue">The value to update in the database.</param>
        public void SetColumnField(string diff, long itemRowIndex, string columnName, bool right, object updatedValue)
        {
            if (right && _rightTable == null)
                throw new ArgumentException("Illegal operation (right table does not exist)");
            else if (!right && _leftTable == null)
                throw new ArgumentException("Illegal operation (left table does not exist)");

            long rowid;
            SQLiteCreateTableStatement table;
            TableChangeItem citem = GetChangeItem(diff, itemRowIndex);
            SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder();
            if (right)
            {
                sb.DataSource = _rightdb;
                table = _rightTable;
                rowid = citem.RightRowId;
            }
            else
            {
                sb.DataSource = _leftdb;
                table = _leftTable;
                rowid = citem.LeftRowId;
            }

            // Open a connection to the left or right databases
            using (SQLiteConnection dbconn = new SQLiteConnection(sb.ConnectionString))
            {
                dbconn.Open();

                SQLiteTransaction tx = dbconn.BeginTransaction();
                try
                {
                    // Disable triggers (should always remain disabled so I don't match this with
                    // another statement to re-enable them)
                    SQLiteCommand disable = new SQLiteCommand(
                        @"PRAGMA DISABLE_TRIGGERS = 1", dbconn, tx);
                    disable.ExecuteNonQuery();

                    // Prepare and execute a command to update the value of the field
                    SQLiteCommand update = BuildUpdateColumnFieldCommand(table, columnName, rowid, updatedValue, dbconn, tx);
                    update.ExecuteNonQuery();

                    // If the column that was changed is a INTEGER PRIMARY KEY column then the corresponding rowid 
                    // was also changed (SQLite uses the INTEGER PRIMARY KEY column to store the RowID in this case)
                    // and we need to update the difference table row so that it points to the updated row id.
                    if (Utils.IsColumnActingAsRowIdAlias(table, columnName))
                    {
                        long updatedRowId = (long)updatedValue;

                        // Update the cached copy's link to the actual database table
                        if (right)
                            citem.RightRowId = updatedRowId;
                        else
                            citem.LeftRowId = updatedRowId;
                    } // if

                    // Update the cached copy field value that was changed
                    if (right)
                    {
                        citem.SetField(columnName, false, updatedValue);

                        // Do the comparison only if the left table contains this column
                        if (Utils.GetColumnByName(_leftTable.Columns, columnName) != null &&
                            (citem.Result == ComparisonResult.Same || citem.Result == ComparisonResult.DifferentData))
                        {
                            object lval = citem.GetField(columnName, true);
                            if (citem.ChangedBlobsColumnNames != null &&
                                updatedValue == DBNull.Value &&
                                (lval == DBNull.Value || (lval is long && ((long)lval) == 0)))
                                citem.ChangedBlobsColumnNames.Remove(columnName);
                        }
                    }
                    else
                    {
                        citem.SetField(columnName, true, updatedValue);

                        // Do the comparison only of the right table contains this column
                        if (Utils.GetColumnByName(_rightTable, columnName) != null && 
                            (citem.Result == ComparisonResult.Same || citem.Result == ComparisonResult.DifferentData))
                        {
                            object rval = citem.GetField(columnName, false);
                            if (citem.ChangedBlobsColumnNames != null &&
                                updatedValue == DBNull.Value &&
                                (rval == DBNull.Value || (rval is long && ((long)rval) == 0)))
                                citem.ChangedBlobsColumnNames.Remove(columnName);
                        } // if
                    }
                    
                    tx.Commit();
                    _precise = false;

                    // Update the change item in the difference database
                    UpdateTableChangeItem(diff, citem, true);
                } // try
                catch (Exception ex)
                {
                    tx.Rollback();
                    throw;
                } // catch
			} // using
        }

        /// <summary>
        /// Causes the table changes object to enter UPDATE mode in which every call to one
        /// of the AddXXX methods below will be performed in an ongoing transaction context
        /// on the differences database file. This is necessary in order to provide adequate
        /// performance because SQLite databases perform very poorly when we have multiple 
        /// INSERT/UPDATE/DELETE operations that are performed in separate transactional 
        /// contexts.
        /// </summary>
        public void BeginUpdate()
        {
            // Close the previous transaction if necessary
            if (_tx != null)
                _tx.Commit();

            _tx = _main.BeginTransaction();

            // Adjust the transaction property for all ADD-CHANGE commands
            _addDifferent.Transaction = _tx;
            _addExistsInLeft.Transaction = _tx;
            _addExistsInRight.Transaction = _tx;
            _addSame.Transaction = _tx;

            // Restart the commit counter so we can automatically commit after
            // every MAX_PENDING_CHANGES changes were added to the database since
            // the last COMMIT that was performed.
            _commitCount = 0;
        }

        /// <summary>
        /// Add a DIFFERENT change to the differences database. This type of change
        /// records the fact that we have two rows that have the same key but have different
        /// data in at least one of their columns.
        /// </summary>
        /// <param name="leftRowId">The RowID of the left database row</param>
        /// <param name="rightRowId">The RowID of the right database row</param>
        /// <param name="changedBlobsColumnNames">Either NULL if no BLOB fields differ between the two
        /// databases or the names of BLOB fields whose values are different between the two databases.</param>
        public void AddDifferent(long leftRowId, long rightRowId, List<string> changedBlobsColumnNames)
        {
            if (_tx == null)
                throw new ApplicationException("illegal transaction state");
            ExecuteAddChangeCommand(_addDifferent, ComparisonResult.DifferentData, leftRowId, rightRowId, changedBlobsColumnNames);
            if (!_hasDifference)
                _hasDifference = true;
        }

        /// <summary>
        /// Add a SAME cahnge to the differences database. This type is not actually a change
        /// because both the row in the left database and the row in the right database are
        /// actually the same (have the same data for all columns), but we still consider this
        /// a change type because many times the user still wants to view these rows as well.
        /// </summary>
        /// <param name="leftRowId">The RowID of the left database row</param>
        /// <param name="rightRowId">the RowID of the right database row</param>
        public void AddSame(long leftRowId, long rightRowId)
        {
            if (_tx == null)
                throw new ApplicationException("illegal transaction state");
            ExecuteAddChangeCommand(_addSame, ComparisonResult.Same, leftRowId, rightRowId, null);
        }

        /// <summary>
        /// Add a EXISTS-IN-LEFT-DATABASE change to the differences database. This type marks
        /// the fact that a specific row exists only in the left database table but not in the
        /// right database table.
        /// </summary>
        /// <param name="leftRowId">The RowID of the row that exists in the left database table.</param>
        public void AddExistsInLeft(long leftRowId)
        {
            if (_tx == null)
                throw new ApplicationException("illegal transaction state");
            ExecuteAddChangeCommand(_addExistsInLeft, ComparisonResult.ExistsInLeftDB, leftRowId, -1, null);
            if (!_hasDifference)
                _hasDifference = true;
        }

        /// <summary>
        /// Add a EXISTS-IN-RIGHT-DATABASE change to the differences database. This type marks
        /// the fact that a specific row exists only in the right database table but not in the
        /// left database table.
        /// </summary>
        /// <param name="rightRowId">The RowID of the row that exists in the right database table.</param>
        public void AddExistsInRight(long rightRowId)
        {
            if (_tx == null)
                throw new ApplicationException("illegal transaction state");
            ExecuteAddChangeCommand(_addExistsInRight, ComparisonResult.ExistsInRightDB, -1, rightRowId, null);
            if (!_hasDifference)
                _hasDifference = true;
        }

        /// <summary>
        /// Cause the TableChanges object to exit UPDATE mode by commiting any pending changes to
        /// the differences database and nullifying the internally held transaction object.
        /// </summary>
        public void EndUpdate()
        {
            if (_tx == null)
                return;
            _tx.Commit();
            _commitCount = 0;
            _tx = null;
        }

        /// <summary>
        /// Called in case of a database failure 
        /// </summary>
        public void RollbackUpdate()
        {
            if (_tx == null)
                return;
            _tx.Rollback();
            _commitCount = 0;
            _tx = null;
        }

        /// <summary>
        /// Check if there is a difference row that matches the right row id in the
        /// specified table name
        /// </summary>
        /// <param name="tblName">The name of the difference table (one of the difference table names
        /// defined as constants in this class).</param>
        /// <param name="rightRowId">The row id of the right database row to check</param>
        /// <returns>TRUE if the specified difference table contains a difference row with the 
        /// specified row id</returns>
        public bool HasRightRowId(string tblName, long rightRowId)
        {
            if (_tx == null)
                throw new ApplicationException("illegal transaction state");
            SQLiteCommand check = new SQLiteCommand("SELECT COUNT(*) FROM " + tblName +
                @" WHERE RightRowID = " + rightRowId, _tx.Connection, _tx);
            long res = (long)check.ExecuteScalar();
            if (res > 0)
                return true;
            return false;
        }

        /// <summary>
        /// Delete the set of rows specified by the <paramref name="rows"/> argument from the database
        /// specified in the <paramref name="left"/> parameter.
        /// </summary>
        /// <param name="diff">The difference table where these rows are located.</param>
        /// <param name="rows">The rows to delete</param>
        /// <param name="left">TRUE indicates that the rows should be deleted from the left database table,
        /// FALSE indicates that the rows should be deleted from the right database table.</param>
        public void DeleteRows(string diff, List<TableChangesRange> rows, bool left, RowsDeletedProgressHandler handler)
        {
            if (rows == null || rows.Count == 0)
                return;

            // Open a writeable connection to the database where rows will be deleted.
            using (SQLiteConnection to = Utils.MakeDbConnection(left ? _leftdb : _rightdb, true))
            {
                to.Open();

                SQLiteTransaction totx = to.BeginTransaction();
                try
                {
                    // Determine how many rows need to be deleted (used for progress notifications)
                    int total = 0;
                    foreach (TableChangesRange range in rows)
                        total += (int)(range.EndRowId - range.StartRowId + 1);

                    // Will store the number of rows that were deleted so far
                    int deleted = 0;

                    // Prepare the delete command
                    SQLiteCommand del = new SQLiteCommand(
                        @"DELETE FROM " + _tableName.ToString() + " WHERE RowID = @rowId", to, totx);
                    del.Parameters.Add("@rowId", DbType.Int64);

                    // Open the difference database for update
                    BeginUpdate();

                    // Disable triggers (should always remain disabled so I don't match this with
                    // another statement to re-enable them)
                    SQLiteCommand disable = new SQLiteCommand(
                        @"PRAGMA DISABLE_TRIGGERS = 1", to, totx);
                    disable.ExecuteNonQuery();

                    // Go over the entire range(s) of deleted rows and delete each and every row
                    foreach (TableChangesRange range in rows)
                    {
                        if (range.StartRowId == range.EndRowId)
                        {
                            DeleteSingleRow(del, diff, range.StartRowId, left, ref totx);
                            deleted++;
                            _precise = false;
                            if (deleted % MAX_PENDING_CHANGES == 0)
                            {
                                totx.Commit();
                                totx = to.BeginTransaction();
                                BeginUpdate();
                            }

                            if (handler != null)
                            {
                                bool cancel = false;
                                handler(deleted, total, ref cancel);
                                if (cancel)
                                    throw new UserCancellationException();
                            }
                        }
                        else
                        {
                            for (long rowid = range.StartRowId; rowid <= range.EndRowId; rowid++)
                            {
                                DeleteSingleRow(del, diff, rowid, left, ref totx);
                                deleted++;
                                _precise = false;
                                if (deleted % MAX_PENDING_CHANGES == 0)
                                {
                                    totx.Commit();
                                    totx = to.BeginTransaction();
                                    BeginUpdate();
                                }

                                if (handler != null)
                                {
                                    bool cancel = false;
                                    handler(deleted, total, ref cancel);
                                    if (cancel)
                                        throw new UserCancellationException();
                                }
                            } // for
                        } // else
                    } // foreach

                    totx.Commit();

                    // Commit all changes done to the difference database
                    EndUpdate();
                }
                catch (Exception ex)
                {
                    totx.Rollback();
                    RollbackUpdate();
                    throw;
                } // catch
			} // using

            // Invalidate the internal cache
            _cache.Clear();
        }

        /// <summary>
        /// Search the specified data table from the specified row index (the row index is used
        /// to locate the table change row from which we need to start the search) and using
        /// the specified <paramref name="sql"/> WHERE clause to locate a matching row.
        /// </summary>
        /// <param name="isLeft">TRUE indicates to search in the left database, FALSE indicates to
        /// search in the right database.</param>
        /// <param name="diff">The difference type table to search</param>
        /// <param name="rowIndex">The row index from which to begin the search (inclusive).</param>
        /// <param name="sql">The SQL WHERE clause to use</param>
        public long SearchRowsBySQL(bool isLeft, string diff, long rowIndex, long maxRowIndex, string sql, SearchRowsProgressHandler handler)
        {
            long total = GetTotalChangesCount(new string[] { diff });
            if (rowIndex >= total)
                return -1;            

            string dbpath = isLeft ? _leftdb : _rightdb;
            using (SQLiteConnection conn = Utils.MakeDbConnection(dbpath, false))
            {
                conn.Open();

                SQLiteCommand cmd = new SQLiteCommand(
                    @"SELECT EXISTS (SELECT * FROM " + _tableName + " WHERE RowID = @rowId AND (" + sql + "))", conn);
                cmd.Parameters.Add("@rowId", DbType.Int64);

                long max = total;
                if (maxRowIndex != -1)
                    max = maxRowIndex + 1;
                for (long index = rowIndex; index < max; index++)
                {
                    if (handler != null)
                    {
                        bool cancel = false;
                        handler(index - rowIndex, total - rowIndex, ref cancel);
                        if (cancel)
                            throw new UserCancellationException();
                    } // if

                    TableChangeItem citem = GetChangeItem(diff, index);
                    long rowId = isLeft ? citem.LeftRowId : citem.RightRowId;
                    if (rowId == -1)
                        continue;
                    cmd.Parameters["@rowId"].Value = rowId;

                    long exists = (long)cmd.ExecuteScalar();
                    if (exists == 1)
                        return index;
                } // for
			} // using

            return -1;
        }

        /// <summary>
        /// Copy the set of rows specified by the <paramref name="rows"/> argument from one database
        /// table to another (determined by the <paramref name="leftToRight"/> argument).
        /// </summary>
        /// <param name="diff">The difference table where these rows are located.</param>
        /// <param name="rows">The rows to copy.</param>
        public void CopyRows(string diff, List<TableChangesRange> rows, bool leftToRight, RowsCopyingProgressHandler handler)
        {
            if (rows == null || rows.Count == 0)
                return;

            // Validate the copy operation and throw exception if a problem is detected
            ValidateRowsCopyOperation(leftToRight);

            // Compute the list of common columns
            List<SQLiteColumnStatement> dcols = null;
            List<SQLiteColumnStatement> common = Utils.GetCommonColumns(
                leftToRight?_rightTable:_leftTable, leftToRight?_leftTable:_rightTable, false, out dcols);

            // Open the database to copy rows from
            using (SQLiteConnection from = Utils.MakeDbConnection(leftToRight ? _leftdb : _rightdb, false))
            {
                from.Open();

                SQLiteTransaction fromtx = from.BeginTransaction(true);

                try
                {
                    // Open the database to copy rows to
                    using (SQLiteConnection to = Utils.MakeDbConnection(leftToRight ? _rightdb : _leftdb, true))
                    {
                        to.Open();

                        SQLiteTransaction totx = to.BeginTransaction();

                        try
                        {
                            // Determine how many rows need to be copied (used for progress notifications)
                            int total = 0;
                            foreach (TableChangesRange range in rows)
                                total += (int)(range.EndRowId - range.StartRowId + 1);

                            // Will store the number of rows that were copied so far
                            int copied = 0;

                            // Open the difference database for update
                            BeginUpdate();

                            // Disable triggers (should always remain disabled so I don't match this with
                            // another statement to re-enable them)
                            SQLiteCommand disable = new SQLiteCommand(
                                @"PRAGMA DISABLE_TRIGGERS = 1", to, totx);
                            disable.ExecuteNonQuery();

                            // Go over the entire range(s) of copied rows and copy each and every row
                            foreach (TableChangesRange range in rows)
                            {
                                if (range.StartRowId == range.EndRowId)
                                {
                                    CopySingleRow(diff, range.StartRowId, leftToRight, common, dcols, fromtx, ref totx, handler);
                                    copied++;
                                    _precise = false;
                                    if (copied % MAX_PENDING_CHANGES == 0)
                                    {
                                        totx.Commit();
                                        totx = to.BeginTransaction();
                                        BeginUpdate();
                                    }

                                    if (handler != null)
                                    {
                                        bool cancel = false;
                                        handler(copied, total, null, 0, 0, ref cancel);
                                        if (cancel)
                                            throw new UserCancellationException();
                                    }
                                }
                                else
                                {
                                    for (long rowid = range.StartRowId; rowid <= range.EndRowId; rowid++)
                                    {
                                        CopySingleRow(diff, rowid, leftToRight, common, dcols, fromtx, ref totx, handler);
                                        copied++;
                                        _precise = false;
                                        if (copied % MAX_PENDING_CHANGES == 0)
                                        {
                                            totx.Commit();
                                            totx = to.BeginTransaction();
                                            BeginUpdate();
                                        }

                                        if (handler != null)
                                        {
                                            bool cancel = false;
                                            handler(copied, total, null, 0, 0, ref cancel);
                                            if (cancel)
                                                throw new UserCancellationException();
                                        }
                                    } // for
                                } // else
                            } // foreach
                        }
                        catch (Exception nex)
                        {
                            totx.Rollback();
                            RollbackUpdate();
                            throw;
                        } // catch

                        totx.Commit();

                        // Commit all changes done to the difference database
                        EndUpdate();
                    } // using
                }
                catch (Exception ex)
                {
                    fromtx.Rollback();
                    throw;
                }

                fromtx.Commit();
			} // using

            // Invalidate the internal cache
            _cache.Clear();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// If TRUE - means that both tables have the same data rows.
        /// </summary>
        public bool SameTables
        {
            get { return !_hasDifference; }
        }
        #endregion

        #region IDisposable
        private void Dispose(bool disposing)
        {
            if (!(this.disposed))
            {
                if (disposing)
                {
                    if (_main != null)
                    {
                        try
                        {
                            //SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder(_main.ConnectionString);
                            string fpath = _main.DataSource;
                            _main.Close();
                            _main.Dispose();
                            _main = null;
                            if (!_inmemory)
                                File.Delete(fpath);
                        }
                        catch (Exception ex)
                        {
                            _log.Error("error occured while cleaning table changes database", ex);
                            throw;
                        } // catch
                    }
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Needed in order to prevent spurious columns in the generated CSV file in case the column name
        /// contains ',' or '"' characters
        /// </summary>
        private static string EscapeColumn(string colname)
        {
            if (colname.Contains(",") || colname.Contains("\""))
                return EscapeToString(colname);
            else
                return colname;
        }

        /// <summary>
        /// Return a list of column names that are common to both database tables
        /// </summary>
        private List<string> GetCommonColumnNames()
        {
            List<string> res = new List<string>();

            List<SQLiteColumnStatement> common = Utils.GetCommonColumns(_leftTable, _rightTable);
            foreach (SQLiteColumnStatement stmt in common)
                res.Add(stmt.ObjectName.ToString());

            return res;
        }

        /// <summary>
        /// Export all changes from a specific difference type.
        /// </summary>
        /// <param name="writer">The StreamWriter to write the changes to</param>
        /// <param name="difftype">The difference type to export</param>
        /// <param name="exported">The total number of changes that were exported</param>
        /// <param name="total">The total number of changes that need to be exported</param>
        /// <param name="handler">A handler for progress notifications</param>
        private void ExportChanges(StreamWriter writer, List<string> common, string difftype, ref long exported, long total, ExportProgressHandler handler)
        {
            long count = GetTotalChangesCount(new string[] { difftype });
            for (long index = 0; index < count; index++)
            {
                TableChangeItem item = GetChangeItem(difftype, index);
                ExportChangeLine(writer, common, item);
                exported++;
                if (handler != null)
                {
                    bool cancel = false;
                    handler(exported, total, ref cancel);
                    if (cancel)
                        throw new UserCancellationException();
                }
            } // for
        }

        /// <summary>
        /// Write a single change item to a file in CSV format
        /// </summary>
        /// <param name="writer">The writer to write to</param>
        /// <param name="item">The item to write</param>
        private void ExportChangeLine(StreamWriter writer, List<string> common, TableChangeItem item)
        {
            string marker;

            if (item.Result == ComparisonResult.DifferentData)
                marker = "UPDATED";
            else if (item.Result == ComparisonResult.ExistsInLeftDB)
                marker = "DELETED";
            else if (item.Result == ComparisonResult.ExistsInRightDB)
                marker = "ADDED";
            else
                marker = "?";

            writer.Write(marker+",");

            if (item.Result == ComparisonResult.ExistsInLeftDB)
            {
                // Write only the values of the left database row
                for (int i = 0; i < common.Count; i++)
                {
                    object obj = item.GetField(common[i], true);
                    writer.Write(EscapeToString(obj));
                    if (i < common.Count - 1)
                        writer.Write(",");
                }
            }
            else if (item.Result == ComparisonResult.ExistsInRightDB)
            {
                // Write only the values of the right database row
                for (int i = 0; i < common.Count; i++)
                {
                    object obj = item.GetField(common[i], false);
                    writer.Write(EscapeToString(obj));
                    if (i < common.Count - 1)
                        writer.Write(",");
                }
            }
            else
            {
                // Write left and right rows
                for (int i = 0; i < common.Count; i++)
                {
                    object obj = item.GetField(common[i], true);
                    writer.Write(EscapeToString(obj));
                    if (i < common.Count - 1)
                        writer.Write(",");
                }

                writer.Write("\r\n  ,");

                // Write the values of the right database row
                for (int i = 0; i < common.Count; i++)
                {
                    object obj = item.GetField(common[i], false);
                    writer.Write(EscapeToString(obj));
                    if (i < common.Count - 1)
                        writer.Write(",");
                }
            } // else
            writer.WriteLine();
        }

        private static string EscapeToString(object obj)
        {
            string str = obj as string;
            if (str != null)
            {
                // Replace all new lines with spaces
                str = str.Replace('\n', ' ');
                str = str.Replace("\"", "\"\"");
                str = "\"" + str + "\"";
                return str;
            }

            if (obj is byte[])
                return "BLOB";

            return obj.ToString();
        }

        /// <summary>
        /// Get the total number of rows in the specified table
        /// </summary>
        /// <param name="dbpath">The path to the database file</param>
        /// <param name="tableName">The name of the table to check</param>
        /// <returns></returns>
        private long GetTableRowsCount(string dbpath, string tableName)
        {
            using (SQLiteConnection conn = Utils.MakeDbConnection(dbpath, false))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(*) FROM " + tableName, conn);
                long count = (long)cmd.ExecuteScalar();
				return count;				
			} // using
        }

        /// <summary>
        /// Deletes a single database table row as specified by the given rowid
        /// </summary>
        /// <param name="del">A SQLiteCommand used to delete the row</param>
        /// <param name="diff">The name of the difference table where the table change item can be found</param>
        /// <param name="rowIndex">The index of the table change row that contains the information about the rows in the
        /// two tables</param>
        /// <param name="left">TRUE indicates deleting from the left database table, FALSE indicates deleting from
        /// the right database table.</param>
        /// <param name="totx">The containing transaction to use</param>
        private void DeleteSingleRow(SQLiteCommand del, string diff, long rowIndex, bool left, ref SQLiteTransaction totx)
        {
            // Get the change item that was specified
            TableChangeItem citem = GetChangeItem(diff, rowIndex);

            // Get the ID of the row to delete
            long deletedRowId = -1;
            if (left)
                deletedRowId = citem.LeftRowId;
            else
                deletedRowId = citem.RightRowId;
            if (deletedRowId == -1)
            {
                // The row does not exist so we can return immediatly without doing anything.
                return;
            }

            // Execute the deletion
            del.Transaction = totx;
            del.Connection = totx.Connection;
            del.Parameters["@rowId"].Value = deletedRowId;
            del.ExecuteNonQuery();

            // Update the change item
            if (left)
            {
                citem.LeftRowId = -1;
                if (citem.RightFields != null)
                    citem.Result = ComparisonResult.ExistsInRightDB;
                else
                    citem.Result = ComparisonResult.Deleted;
            }
            else
            {
                citem.RightRowId = -1;
                if (citem.LeftFields != null)
                    citem.Result = ComparisonResult.ExistsInLeftDB;
                else
                    citem.Result = ComparisonResult.Deleted;                
            }
            citem.ChangedBlobsColumnNames = null;
            UpdateTableChangeItem(diff, citem, false);
        }

        /// <summary>
        /// Validate that we can copy rows from the source table to the target table.
        /// </summary>
        /// <param name="leftToRight">TRUE means that rows will be copied from LEFT database to the RIGHT database.</param>
        private void ValidateRowsCopyOperation(bool leftToRight)
        {
            // Compute a subset of columns that can be copied between the two tables
            List<SQLiteColumnStatement> common = Utils.GetCommonColumns(_leftTable, _rightTable);

            // Validate that we can perform the copying
            if (common.Count == 0)
                throw new ApplicationException("No rows can be copied since the two tables have no shared\r\n" +
                    "columns. Copy cannot continue.");

            SQLiteCreateTableStatement targetTable = leftToRight ? _rightTable : _leftTable;

            // Validate that all primary keys of the target table can be copied from the source
            // table.
            List<SQLiteColumnStatement> pkeys = Utils.GetPrimaryColumns(targetTable);
            foreach (SQLiteColumnStatement pk in pkeys)
            {
                if (!Utils.ColumnListContains(common, pk))
                {
                    // We should be able to copy all primary keys of the target table from
                    // the source table. If this is not the case - issue an error and abort
                    // the operation.
                    throw new ApplicationException("One of the primary keys of the target table " +
                        "is missing from the source table. Copy cannot continue.");
                }
            } // foreach

            // Validate that all columns that are exclusive to the target table (that are not part
            // of the common columns list) are either declared as NULL or have a DEFAULT clause whose
            // value is not null.
            foreach (SQLiteColumnStatement col in targetTable.Columns)
            {
                if (!Utils.ColumnListContains(common, col))
                {
                    if (!col.IsNullable && !col.HasNonNullDefault)
                    {
                        // The column is NOT NULL and does not have a suitable (non-null)
                        // DEFAULT clause so INSERTing rows will fail
                        throw new ApplicationException("The target table contains a column (" + col.ObjectName.ToString() + ")" +
                            " that is declared as NOT NULL but it doesn't have a suitable DEFAULT clause.\r\n" +
                            "This may cause INSERT operations to fail. Copy cannot continue.");
                    }
                } // if
            } // foreach
        }

        /// <summary>
        /// Copy the database row specified by the ROWID of the table change item row from 'from' database table
        /// to 'to' database table.
        /// </summary>
        /// <param name="diff">The difference table where the rowid pointed by <paramref name="rowid"/> can be found</param>
        /// <param name="rowIndex">The row index of the table change item row that we are going to use in order to locate the
        /// rowID of the left row and the rowid of the right row to copy.</param>
        /// <param name="leftToRight">TRUE means that copying will be done from the left database table to the right database table</param>
        /// <param name="from">The connection of the database from which the copy will be performed.</param>
        /// <param name="to">The connection of the database to which the copy will be performed.</param>
        private void CopySingleRow(string diff, long rowIndex, bool leftToRight, List<SQLiteColumnStatement> common, 
            List<SQLiteColumnStatement> dcols,
            SQLiteTransaction fromtx, ref SQLiteTransaction totx, RowsCopyingProgressHandler handler)
        {
            // Get the change item that was specified
            TableChangeItem citem = GetChangeItem(diff, rowIndex);

            // Determine the 'FROM' row id and the 'TO' rowid for the copy
            long fromRowId;
            long toRowId;
            if (leftToRight)
            {
                fromRowId = citem.LeftRowId;
                toRowId = citem.RightRowId;
            }
            else
            {
                fromRowId = citem.RightRowId;
                toRowId = citem.LeftRowId;
            }

            // Prepare the INSERT command
            SQLiteCommand insert = new SQLiteCommand(
				$@"
				INSERT INTO {_tableName}
					({Utils.BuildColumnsString(common, false)})
				VALUES
					({Utils.BuildColumnParametersString(common)});
				SELECT last_insert_rowid()
				",
                totx.Connection, totx);
            Utils.AddCommandColumnParameters(insert, common);

            // Prepare the UPDATE command

            // Prepare the DELETE command
            SQLiteCommand del = new SQLiteCommand($"DELETE FROM {_tableName} WHERE RowID = @rowId", totx.Connection, totx);
            del.Parameters.Add("@rowId", DbType.Int64);

            // There are 3 scenarios that need to be dealt with:
            // 1. The source database contains a row but the target database doesn't
            // 2. The source database contains a row and the target database contains a row
            // 3. The source database doesn't contain a row and the target database contains a row
            if (citem.Result == ComparisonResult.ExistsInLeftDB || citem.Result == ComparisonResult.ExistsInRightDB)
            {
                if (toRowId == -1)
                {
                    // If the target table does not have a row - we'll need to INSERT a new row
                    InsertRowFrom(insert, citem, fromRowId, fromtx, ref totx, common, leftToRight, handler);
                }
                else
                {
                    // If the target table does have a row - we'll have to delete it
                    DeleteRowIn(del, citem, ref totx, toRowId, leftToRight);
                }

                // Update the change item to reflect the insertion
                UpdateTableChangeItem(diff, citem, false);                    
            }
            else if (citem.Result == ComparisonResult.DifferentData)
            {
                // We'll have to UPDATE the existing row in the target database table
                UpdateRowIn(citem, fromRowId, toRowId, ref totx, common, dcols, leftToRight, handler);

                // Update the change item to reflect the insertion
                UpdateTableChangeItem(diff, citem, false);                    
            }
        }

        /// <summary>
        /// Update the target row from the source row with all common fields.
        /// </summary>
        /// <param name="citem">The table change item that points to the source and target rows</param>
        /// <param name="fromRowId">The RowID of the source row</param>
        /// <param name="toRowId">The ROwID of the target row</param>
        /// <param name="totx">The transaction of the target database</param>
        /// <param name="common">The list of common columns that can be copied</param>
        /// <param name="leftToRight">TRUE indicates left-to-right copying, FALSE indicates right-to-left copying</param>
        private void UpdateRowIn(TableChangeItem citem, long fromRowId, long toRowId,
            ref SQLiteTransaction totx, List<SQLiteColumnStatement> common,
            List<SQLiteColumnStatement> dcols,
            bool leftToRight, RowsCopyingProgressHandler handler)
        {
            // This list will contain all blob columns that need to be overwritten using the BLOB 
            // chunking API
            List<SQLiteColumnStatement> blobs = null;

            SQLiteCommand update = new SQLiteCommand();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < common.Count; i++)
            {
                string colName = common[i].ObjectName.ToString();
                string prmName = Utils.GetColumnParameterName(colName);
                sb.Append(colName + " = " + prmName);
                DbType dsttype = Utils.GetDbType(common[i].ColumnType);
                update.Parameters.Add(prmName, dsttype);

                SQLiteColumnStatement origCol = null;
                if (dcols != null)
                    origCol = Utils.GetColumnByName(dcols, colName);
                DbType srctype = dsttype;
                if (origCol != null)
                    srctype = Utils.GetDbType(origCol.ColumnType);

                // We have special treatment for BLOB fields - We'll set them to NULL if they are null in
                // the source table, or we'll set them to a dummy non-null value and later
                // use the BLOB chunking API to write them to the target row.
                object value = citem.GetField(SQLiteParser.Utils.Chop(colName), leftToRight);
                if (srctype == DbType.Binary)
                {
                    // Since the actual value that gets stored in the table change item is either 0 (when the BLOB field is null)
                    // or 1 (when the BLOB field is not null) - we'll have to translate this when inserting the row to the
                    // target table.
                    long notnull = (long)value;
                    if (notnull == 1)
                    {
                        if (dsttype == DbType.Binary)
                        {
                            Guid id = Guid.NewGuid();
                            value = id.ToByteArray(); // Dummy value made from GUID in order to prevent possible UNIQUEness errors
                            if (blobs == null)
                                blobs = new List<SQLiteColumnStatement>();
                            blobs.Add(common[i]);
                        }
                        else
                        {
                            // Another special case.. Here we can't simply copy the value that was retrieved
                            // from the change item field because it only marks if the BLOB field is NULL or not.
                            // We'll need to read the entire BLOB field into main memory and convert it to the
                            // type of the destination column.
                            using (BlobReaderWriter brw = new BlobReaderWriter(leftToRight ? _leftdb : _rightdb, true))
                            {
                                value = brw.ReadBlob(_tableName.ToString(), colName, fromRowId);
                            } // using
                        } // else
                    }
                    else
                    {
                        value = DBNull.Value;
                    }
                } // if

                Utils.AssignParameterValue(update.Parameters[prmName], value);

                if (i < common.Count - 1)
                    sb.Append(", ");
            } // for
            update.Parameters.Add("@rowId", DbType.Int64);
            update.Parameters["@rowId"].Value = toRowId;
            update.CommandText = @"UPDATE " + _tableName.ToString() + " SET " + sb.ToString() + " WHERE RowID = @rowId";
            update.Connection = totx.Connection;
            update.Transaction = totx;

            // Execute the UPDATE command
            update.ExecuteNonQuery();

            // Overwrite non-null BLOB fields using the chunking API
            OverwriteNonNullBlobs(ref totx, blobs, fromRowId, toRowId, leftToRight, handler);

            // Update the table change item
            citem.ChangedBlobsColumnNames = null;
            citem.Result = ComparisonResult.Same;
        }

        /// <summary>
        /// Called after updating or inserting a row with non-null BLOB fields in order to overwrite the dummy
        /// values that were written to these field with the actual BLOB field value (taken from the source row).
        /// </summary>
        /// <param name="blobs">A list of columns where non-null BLOB fields were found</param>
        /// <param name="fromRowId">The row id of the source row</param>
        /// <param name="toRowId">The row id of the target row</param>
        /// <param name="leftToRight">TRUE to indicate left-to-right copying, FALSE to indicate right-to-left copying.</param>
        private void OverwriteNonNullBlobs(ref SQLiteTransaction tx, List<SQLiteColumnStatement> blobs, 
            long fromRowId, long toRowId, bool leftToRight, RowsCopyingProgressHandler handler)
        {
            // Overwrite non-null BLOB fields using the chunking API
            if (blobs != null)
            {
                // Close the existing transaction
                SQLiteConnection conn = tx.Connection;
                tx.Commit();

                foreach (SQLiteColumnStatement blobColumn in blobs)
                {
                    string blobName = blobColumn.ObjectName.ToString();
                    using (BlobReaderWriter brw = new BlobReaderWriter(leftToRight ? _leftdb : _rightdb, true))
                    {
                        BlobProgressHandler bph = new BlobProgressHandler(delegate(int bytesRead, int totalBytes, ref bool cancel)
                        {
							handler?.Invoke(-1, -1, blobName, bytesRead, totalBytes, ref cancel);
                        });

                        brw.CopyBlob(leftToRight ? _rightdb : _leftdb, 
                            SQLiteParser.Utils.Chop(_tableName.ToString()), 
                            SQLiteParser.Utils.Chop(blobColumn.ObjectName.ToString()), fromRowId, toRowId, bph);
                    } // using
                } // foreach

                // Re-open the transaction
                tx = conn.BeginTransaction();
            } // if
        }

        private void DeleteRowIn(SQLiteCommand del, TableChangeItem citem, ref SQLiteTransaction totx, long toRowId, bool leftToRight)
        {
            del.Parameters["@rowId"].Value = toRowId;
            del.ExecuteNonQuery();

            // Update the table change item
            if (leftToRight)
                citem.RightRowId = -1;
            else
                citem.LeftRowId = -1;
            citem.Result = ComparisonResult.Deleted;
            citem.ChangedBlobsColumnNames = null;
        }

        private void UpdateTableChangeItem(string diff, TableChangeItem citem, bool commit)
        {
            if (commit)
                BeginUpdate();
            try
            {
                SQLiteCommand updateItem = new SQLiteCommand(
                    "UPDATE " + diff + " SET RightRowID = @rightRowId, LeftRowID = @leftRowId, ComparisonResult = @result, ChangedBlobs = @changedBlobs WHERE RowID = @rowId",
                    _main, _tx);
                updateItem.Parameters.Add("@rightRowId", DbType.Int64);
                updateItem.Parameters["@rightRowId"].Value = citem.RightRowId;
                updateItem.Parameters.Add("@leftRowId", DbType.Int64);
                updateItem.Parameters["@leftRowId"].Value = citem.LeftRowId;
                updateItem.Parameters.Add("@result", DbType.Int64);
                updateItem.Parameters["@result"].Value = (long)citem.Result;
                updateItem.Parameters.Add("@rowId", DbType.Int64);
                updateItem.Parameters["@rowId"].Value = citem.ChangeItemRowId;
                updateItem.Parameters.Add("@changedBlobs", DbType.String);
                if (citem.ChangedBlobsColumnNames != null)
                    updateItem.Parameters["@changedBlobs"].Value = Serialize(citem.ChangedBlobsColumnNames);
                else
                    updateItem.Parameters["@changedBlobs"].Value = DBNull.Value;
                updateItem.ExecuteNonQuery();

                if(commit)
                    EndUpdate();
            }
            catch (Exception nex)
            {
                if (_tx != null)
                    _tx.Rollback();
                throw;
            } // catch
        }

        private void InsertRowFrom(SQLiteCommand insert, TableChangeItem citem, long fromRowId,
            SQLiteTransaction fromtx, ref SQLiteTransaction totx, 
            List<SQLiteColumnStatement> common, bool leftToRight, RowsCopyingProgressHandler handler)
        {
            // This list will contain all blob columns that need to be overwritten using the BLOB 
            // chunking API
            List<SQLiteColumnStatement> blobs = null;

            // Fill INSERT values from the change item
            for (int i = 0; i < common.Count; i++)
            {
                string pname = Utils.GetColumnParameterName(common[i].ObjectName.ToString());

                // We have special treatment for BLOB fields - we'll first set them to NULL and then we'll 
                // use the BLOB chunking API to write them to the target row.
                object value = citem.GetField(SQLiteParser.Utils.Chop(common[i].ObjectName.ToString()), leftToRight);
                if (Utils.GetDbType(common[i].ColumnType) == DbType.Binary)
                {
                    // Since the actual value that gets stored in the table change item is either 0 (when the BLOB field is null)
                    // or 1 (when the BLOB field is not null) - we'll have to translate this when inserting the row to the
                    // target table.
                    long notnull = (long)value;
                    if (notnull == 1)
                    {
                        Guid id = Guid.NewGuid();
                        value = id.ToByteArray(); // Dummy value made from GUID in order to prevent possible UNIQUEness errors
                        if (blobs == null)
                            blobs = new List<SQLiteColumnStatement>();
                        blobs.Add(common[i]);
                    }
                    else
                    {
                        value = DBNull.Value;
                    }
                }

				Utils.AssignParameterValue(insert.Parameters[pname], value);
            } // for

            // Execute the insertion and get the ROWID of the inserted row.
            long insertRowId = (long)insert.ExecuteScalar();

            // Overwrite non-null BLOB fields using the chunking API
            OverwriteNonNullBlobs(ref totx, blobs, fromRowId, insertRowId, leftToRight, handler);

            // Update the table change item to reflect the insertion
            if (leftToRight)
                citem.RightRowId = insertRowId;
            else
                citem.LeftRowId = insertRowId;
            citem.ChangedBlobsColumnNames = null;
            citem.Result = ComparisonResult.Same;
        }

        private SQLiteCommand BuildUpdateColumnFieldCommand(SQLiteCreateTableStatement table, string columnName,
            long rowid, object updatedValue, SQLiteConnection dbconn, SQLiteTransaction tx)
        {
            SQLiteColumnStatement column = Utils.FindColumn(table.Columns, columnName);

            SQLiteCommand res = new SQLiteCommand(
                @"UPDATE " + table.ObjectName.ToString() + " SET " + column.ObjectName.ToString() +
                " = @updated WHERE RowID = @rowid",
                dbconn, tx);
            DbType dbtype = Utils.GetDbTypeFromClrType(column, updatedValue);
            if (dbtype == DbType.Single)
            {
                // Special handling in order to prevent float->double conversion inaccuracies
                updatedValue = Math.Round((double)(float)updatedValue, 7, MidpointRounding.AwayFromZero);
                dbtype = DbType.Double;
            }
            res.Parameters.Add("@updated", dbtype);
            res.Parameters.Add("@rowid", System.Data.DbType.Int64);

            res.Parameters["@updated"].Value = updatedValue;
            res.Parameters["@rowid"].Value = rowid;

            return res;
        }

        /// <summary>
        /// Prepare a SELECT statement to retrieve the fields of the current row.
        /// </summary>
        /// <remarks>We prevent loading BLOB fields by replacing them with calls to IS NULL expression
        /// that returns 0 if they are NULL or 1 if they are not. This is done because BLOB fields can be extremely large
        /// and may cause the application to slow or even crash due to memory problems.</remarks>
        /// <param name="table">The table for which we build the SELECT command</param>
        /// <param name="conn">The connection to use for retrieving the row</param>
        /// <returns>The requested command</returns>
        private SQLiteCommand PrepareReadRowCommand(SQLiteCreateTableStatement table, SQLiteConnection conn)
        {
            // We need to prepare a list of columns to be read from the table.
            // In order to avoid loading large amounts of data when dealing with BLOB columns,
            // we'll replace these columns by a corresponding BLOB IS NULL alias.
            string clist = Utils.BuildColumnsString(table.Columns, true);
            SQLiteCommand res = new SQLiteCommand(
                @"SELECT " + clist + " FROM " + table.ObjectName.ToString() + " WHERE RowID = @rowId", conn);
            res.Parameters.Add("@rowId", DbType.Int64);

            return res;
        }

        /// <summary>
        /// Build a new change item based on the specified result and values in the left row
        /// and right rows.
        /// </summary>
        /// <param name="cres">The comparison result</param>
        /// <param name="readLeftRow">The command to read the values stored in the left row</param>
        /// <param name="leftRowId">The rowid for the left row</param>
        /// <param name="readRightRow">The command to read the values stored in the right row</param>
        /// <param name="rightRowId">The rowid for the right row</param>
        /// <returns>The change item</returns>
        private TableChangeItem BuildChangeItem(ComparisonResult cres,
            SQLiteCommand readLeftRow, long leftRowId,
            SQLiteCommand readRightRow, long rightRowId)
        {
            TableChangeItem res = new TableChangeItem();
            res.Result = cres;
            res.LeftRowId = leftRowId;
            res.RightRowId = rightRowId;
            FillItemValues(res, readLeftRow, leftRowId, true);
            FillItemValues(res, readRightRow, rightRowId, false);

            return res;
        }

        /// <summary>
        /// Convenience method
        /// </summary>
        /// <param name="item">The change item object</param>
        /// <param name="cmd">The SQLite comamnd to execute</param>
        /// <param name="rowId">The row id</param>
        /// <param name="isLeft">TRUE means left values, FALSE means right values</param>
        private void FillItemValues(TableChangeItem item, SQLiteCommand cmd, long rowId, bool isLeft)
        {
            if (rowId != -1)
            {
                cmd.Parameters["@rowId"].Value = rowId;
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        if (isLeft)
                        {
                            item.LeftFields = new object[reader.FieldCount];
							reader.GetValues().CopyTo(item.LeftFields, 0);

                            if (_leftColNames == null)
                            {
                                _leftColNames = new string[reader.FieldCount];
                                for (int i = 0; i < _leftColNames.Length; i++)
                                    _leftColNames[i] = reader.GetName(i);
                            }

                            if (_leftFieldIndexes == null)
                            {
                                _leftFieldIndexes = new Dictionary<string, int>();
                                for (int i = 0; i < _leftColNames.Length; i++)
                                    _leftFieldIndexes.Add(_leftColNames[i].ToLower(), i);
                            }

                            item.LeftColumnNames = _leftColNames;
                            item.LeftFieldIndexes = _leftFieldIndexes;
                        }
                        else
                        {
                            item.RightFields = new object[reader.FieldCount];
							reader.GetValues().CopyTo(item.RightFields, 0);

							if (_rightColNames == null)
                            {
                                _rightColNames = new string[reader.FieldCount];
                                for (int i = 0; i < _rightColNames.Length; i++)
                                    _rightColNames[i] = reader.GetName(i);
                            }

                            if (_rightFieldIndexes == null)
                            {
                                _rightFieldIndexes = new Dictionary<string, int>();
                                for (int i = 0; i < _rightColNames.Length; i++)
                                    _rightFieldIndexes.Add(_rightColNames[i].ToLower(), i);
                            }

                            item.RightColumnNames = _rightColNames;
                            item.RightFieldIndexes = _rightFieldIndexes;
                        } // else

                        // Upgrade all integer fields to LONG type.
                        UpgradeItemFields(item);
                    } // if
                } // using
            } // if
        }

        private void UpgradeItemFields(TableChangeItem item)
        {
            UpgradeFields(item.LeftFields);
            UpgradeFields(item.RightFields);
        }

        /// <summary>
        /// Upgrade the item fields if necessary
        /// </summary>
        /// <param name="fields"></param>
        private void UpgradeFields(object[] fields)
        {
            if (fields == null)
                return;
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i] is long)
                    continue;

                if (fields[i] is byte[])
                    fields[i] = Encoding.ASCII.GetString((byte[])fields[i]);
                else if (fields[i] is sbyte)
                    fields[i] = (long)(sbyte)fields[i];
                else if (fields[i] is byte)
                    fields[i] = (long)(byte)fields[i];
                else if (fields[i] is short)
                    fields[i] = (long)(short)fields[i];
                else if (fields[i] is ushort)
                    fields[i] = (long)(ushort)fields[i];
                else if (fields[i] is int)
                    fields[i] = (long)(int)fields[i];
                else if (fields[i] is uint)
                    fields[i] = (long)(uint)fields[i];
                else if (fields[i] is ulong)
                    fields[i] = (long)(ulong)fields[i];
                else if (fields[i] is decimal)
                    fields[i] = (double)(decimal)fields[i];
            } // for
        }

        /// <summary>
        /// Parse the specified string and extract the list column names that 
        /// are embedded in it.
        /// </summary>
        /// <param name="str">A string that contains the names of table columns.</param>
        /// <returns>A list that contains the names of these columns.</returns>
        private List<string> Deserialize(string str)
        {
            string[] parts = str.Split(new string[] { NAME_DELIMITER }, StringSplitOptions.RemoveEmptyEntries);
            return new List<string>(parts);
        }

        /// <summary>
        /// Stringify the specified list of BLOB column names so they can
        /// safely be stored in the difference table database.
        /// </summary>
        /// <param name="changedBlobsColumnNames">A list of BLOB columns whose
        /// values differ between the two databases.</param>
        /// <returns>A stringified representation of the specified column names</returns>
        private string Serialize(List<string> changedBlobsColumnNames)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < changedBlobsColumnNames.Count; i++)
            {
                sb.Append(changedBlobsColumnNames[i]);
                if (i < changedBlobsColumnNames.Count - 1)
                    sb.Append(NAME_DELIMITER);
            } // for
            return sb.ToString();
        }

        /// <summary>
        /// Opens a new SQLite connection to the specified database as readonly
        /// </summary>
        /// <param name="dbfile">The path to the DB file</param>
        /// <returns>The connection</returns>
        private SQLiteConnection MakeReadOnlyConnection(string dbfile)
        {
            SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder();
            sb.DataSource = dbfile;
            sb.ReadOnly = true;
            return new SQLiteConnection(sb.ConnectionString);
        }

        /// <summary>
        /// Build a select command that retrieves all diff table rows for the relevant
        /// difference types.
        /// </summary>
        /// <param name="diffType">The type of difference to show</param>
        /// <param name="limit">The rows limit to retrieve</param>
        /// <param name="offset">The offset</param>
        /// <param name="main">The connection to the table differences database</param>
        /// <returns>The SQLite command necessary for performing the query</returns>
        private SQLiteCommand BuildSelectChangesCommand(
            string diffType, int limit, long offset, SQLiteConnection main)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT RowID, ComparisonResult, LeftRowID, RightRowID, ChangedBlobs FROM " + diffType);
            sb.Append(" WHERE RowID BETWEEN " + offset + " AND " + (offset + limit));

            SQLiteCommand res = new SQLiteCommand(sb.ToString(), main);
            return res;
        }

        /// <summary>
        /// Convenience method for adding a change to a difference table.
        /// </summary>
        /// <param name="add">The command to execute</param>
        /// <param name="result">The row comparison result.</param>
        /// <param name="leftRowId">The ROW-ID of the row in the left database</param>
        /// <param name="rightRowId">The ROW-ID of the row in the right database</param>
        private void ExecuteAddChangeCommand(SQLiteCommand add, ComparisonResult result, long leftRowId, long rightRowId, 
            List<string> changedBlobsColumnNames)
        {
            if (changedBlobsColumnNames == null)
                add.Parameters["@blobs"].Value = DBNull.Value;
            else
            {
                string tmp = Serialize(changedBlobsColumnNames);
                add.Parameters["@blobs"].Value = tmp;
            } // else

            add.Parameters["@cres"].Value = (long)result;
            add.Parameters["@left"].Value = leftRowId;
            add.Parameters["@right"].Value = rightRowId;
            add.ExecuteNonQuery();

            // Automatically commit after every MAX_PENDING_CHANGES changes are added
            _commitCount++;
            if (_commitCount % MAX_PENDING_CHANGES == 0)
            {
                _tx.Commit();
                _commitCount = 0;
                _tx = _main.BeginTransaction();
            }
        }

        /// <summary>
        /// Auxiliary method for creating INSERT command to a specific difference table.
        /// </summary>
        /// <param name="tableName">The different type table.</param>
        /// <returns>The resulting SQL INSERT command.</returns>
        private SQLiteCommand CreateAddCommand(string tableName)
        {
            SQLiteCommand res = new SQLiteCommand(
                @"INSERT INTO " + tableName + " (ComparisonResult, LeftRowID, RightRowID, ChangedBlobs) " +
                @"VALUES (@cres, @left, @right, @blobs)", _main);
            res.Parameters.Add("@cres", System.Data.DbType.Int64);
            res.Parameters.Add("@left", System.Data.DbType.Int64);
            res.Parameters.Add("@right", System.Data.DbType.Int64);
            res.Parameters.Add("@blobs", System.Data.DbType.String);

            return res;
        }

        /// <summary>
        /// Create a temporary table changes database file for the specified table.
        /// This database will be used to store difference information that is accumulated
        /// while computing table data differences between the two databases.
        /// The database contains 4 separate tables that are used to store results for the
        /// 4 possible comparison results. This will make it easy for the GUI to display
        /// subsets of the differences found according to the comparison result.
        /// </summary>
        /// <param name="tableName">The table name for which we create the table changes database.</param>
        /// <returns>An opened SQLite connection object to the fresh changes database file.</returns>
        private SQLiteConnection CreateTableChangesDB(string tableName)
        {
            string fpath = MakeTempDbName(tableName);
            SQLiteConnection.CreateFile(fpath);
            SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder();
            sb.DataSource = fpath;
            sb.PageSize = 4096;

            SQLiteConnection main = new SQLiteConnection(sb.ConnectionString);
            main.Open();

            CreateDifferenceDbSchema(main);

            return main;
        }

        /// <summary>
        /// Creates the schema tables for the differences database
        /// </summary>
        /// <param name="main">The DB connection to use</param>
        private void CreateDifferenceDbSchema(SQLiteConnection main)
        {
            SQLiteCommand create = BuildDiffTable(EXISTS_IN_LEFT_TABLE_NAME, main);
            create.ExecuteNonQuery();
            create = BuildDiffTable(EXISTS_IN_RIGHT_TABLE_NAME, main);
            create.ExecuteNonQuery();
            create = BuildDiffTable(DIFFERENT_ROWS_TABLE_NAME, main);
            create.ExecuteNonQuery();
            create = BuildDiffTable(SAME_ROWS_TABLE_NAME, main);
            create.ExecuteNonQuery();
        }

        /// <summary>
        /// Creates the DDL command that is needed to create the differences database
        /// for the specified table.
        /// </summary>
        /// <param name="tableName">The name of the table for which we are creating
        /// the differences database.</param>
        /// <param name="main">The connection to the differences database.</param>
        /// <returns>A command that can be used to create the differences table</returns>
        private SQLiteCommand BuildDiffTable(string tableName, SQLiteConnection main)            
        {
            SQLiteCommand res = new SQLiteCommand(
                @"CREATE TABLE [" + tableName + "] (" +
                @" LeftRowID integer NOT NULL, " +
                @" RightRowID integer NOT NULL, " +
                @" ComparisonResult integer NOT NULL, " +
                @" ChangedBlobs text"+
                @");"+
                @"CREATE INDEX "+tableName+"_RightRowID_index ON "+tableName+" (RightRowID);", main);
            return res;
        }

        /// <summary>
        /// Used to create a temporary database name that includes the name of the
        /// table for which we want to create the differences database.
        /// </summary>
        /// <param name="tableName">The name of the table to use for constructing the temporary file name</param>
        /// <returns>A valid name that will be used to create the temporary differences database file.</returns>
        private string MakeTempDbName(string tableName)
        {
            string bpath = GetTempFolder();

            string fpath;
            long utc = DateTime.Now.ToBinary();
            if (utc < 0)
                utc = utc * -1;
            tableName = Utils.NormalizeName(tableName+"_G_"+_appid.ToString()+"_G_");
            do
            {    
                fpath = bpath+tableName + utc + ".db";
                utc++;
            } while (File.Exists(fpath));

            return fpath;
        }

        /// <summary>
        /// Returns the path to a temporary folder that is used to hold change files.
        /// </summary>
        /// <returns></returns>
        private static string GetTempFolder()
        {
            string bpath = Path.GetTempPath()+"\\sqlitecompare\\";
            if (!Directory.Exists(bpath))
                Directory.CreateDirectory(bpath);
            return bpath;
        }
        #endregion

        #region Constants
        private const int CACHE_BUFFER_SIZE = 2000;
        private const string NAME_DELIMITER = @"$D@E#L%I^M$";
        #endregion

        #region Private Variables

        private SQLiteCommand _addSame;
        private SQLiteCommand _addDifferent;
        private SQLiteCommand _addExistsInLeft;
        private SQLiteCommand _addExistsInRight;
		private SQLiteCommand _addBoth;

        private Dictionary<string, int> _leftFieldIndexes = null;
        private Dictionary<string, int> _rightFieldIndexes = null;
        private string _leftdb;
        private string _rightdb;
        private bool disposed;
        private string[] _leftColNames;
        private string[] _rightColNames;
        private long _commitCount = 0;
        private bool _hasDifference = false;
        private SQLiteConnection _main;
        private SQLiteTransaction _tx;
        private SQLiteObjectName _tableName;
        private SQLiteCreateTableStatement _leftTable;
        private SQLiteCreateTableStatement _rightTable;
        private static Guid _appid = Guid.NewGuid();
        private static Mutex _appmutex;
        private bool _precise = true;
        private static Regex _tchangeRx = new Regex(@"_G_([0-9A-Fa-f_]+)_G_");
        private static List<TableChanges> _activeChanges = new List<TableChanges>();
        private ILog _log = LogManager.GetLogger(typeof(TableChanges));

        private string _cachediff;
        private long _cacheOffset;
        private List<TableChangeItem> _cache = new List<TableChangeItem>();

        /// <summary>
        /// Indicates if the differences table is stored in a temporary database file or in-memory.
        /// </summary>
        private bool _inmemory = false;
        #endregion
    }

    /// <summary>
    /// Called during rows copying in order to notify about the progress of the operation
    /// </summary>
    /// <param name="rowsCopied">How many rows were already copied</param>
    /// <param name="totalRows">The total number of rows that needs to be copied</param>
    /// <param name="blobName">If non-null - the name of the BLOB field that is currently copied</param>
    /// <param name="blobBytesCopied">The number of BLOB bytes that were copied so far</param>
    /// <param name="totalBlobBytes">The total number of BLOB bytes that need to be copied</param>
    /// <param name="cancel">If set to TRUE it will cause the CopyRows method to throw a UserCancelledException and terminate
    /// the copying process.</param>
    /// <remarks>If the <paramref name="blobName"/> parameter is not null - you should ignore the values stored in the
    /// <paramref name="rowsCopied"/> and <paramref name="totalRows"/> fields.</remarks>
    public delegate void RowsCopyingProgressHandler(int rowsCopied, int totalRows, string blobName, int blobBytesCopied, int totalBlobBytes, ref bool cancel);

    /// <summary>
    /// Called during rows deletion in order to notify about the progress of the operation
    /// </summary>
    /// <param name="rowsDeleted">The number of rows that were deleted</param>
    /// <param name="totalRows">The total number of rows that should be deleted</param>
    /// <param name="cancel">If set to TRUE it will cause the CopyRows method to throw a UserCancelledException and terminate
    /// the copying process.</param>
    public delegate void RowsDeletedProgressHandler(int rowsDeleted, int totalRows, ref bool cancel);

    /// <summary>
    /// Called during rows search in order to notify about the progress of the operation
    /// </summary>
    /// <param name="searchedRows">The number of rows that were searched so far</param>
    /// <param name="total">The total number of rows to search</param>
    /// <param name="cancel">If set to TRUE it will cause the CopyRows method to throw a UserCancelledException and terminate
    /// the copying process.</param>
    public delegate void SearchRowsProgressHandler(long searchedRows, long total, ref bool cancel);

    /// <summary>
    /// Called during export process in order to notify about the progress of the operation.
    /// </summary>
    /// <param name="rowsExported">The number of rows that were already exported</param>
    /// <param name="total">The total number of rows to export</param>
    /// <param name="cancel">If set to TRUE it will cause the ExportToCSV method to throw a UserCancelledException and terminate
    /// the exporting process.</param>
    public delegate void ExportProgressHandler(long rowsExported, long total, ref bool cancel);

    public struct TableChangesRange
    {
        public TableChangesRange(long start, long end)
        {
            if (start > end)
            {
                StartRowId = end;
                EndRowId = start;
            }
            else
            {
                StartRowId = start;
                EndRowId = end;
            }
        }

        public long StartRowId;
        public long EndRowId;
    }
}
