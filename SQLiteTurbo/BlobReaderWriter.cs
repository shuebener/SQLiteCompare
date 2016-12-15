using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Data.SQLite;
using Common;

namespace SQLiteTurbo
{
    /// <summary>
    /// This class provides utility methods for accessing BLOB fields in a way that
    /// doesn't force their contents to be loaded into main memory (aka BLOB streaming).
    /// </summary>
    /// <remarks>This class is not thread-safe. You should never call WriteBlob/ReadBlob on the same
    /// instance from two different threads simultaneously.</remarks>
    public class BlobReaderWriter : IDisposable
    {
        #region Constructors & Destructors
        /// <summary>
        /// Initialize the BlobReaderWriter object using the specified database file path.
        /// </summary>
        /// <param name="dbpath">The path to the database file that contains BLOB fields</param>
        public BlobReaderWriter(string dbpath, bool readOnly)
        {
            // Form a connection to the database file
            if (readOnly)
                _errorCode = sqlite3_open_v2(dbpath, ref _conn1, SQLITE_OPEN_READONLY, null);
            else
                _errorCode = sqlite3_open(dbpath, ref _conn1);
            if (_errorCode > 0)
                throw new SQLiteException(_errorCode, "can't open file: " + dbpath);
        }

        /// <summary>
        /// Initialize a dual database BlobReaderWriter object using the two database files
        /// that are specified in the parameters.
        /// </summary>
        /// <param name="dbpath1">The path to the first database file</param>
        /// <param name="dbpath2">The path to the second database file</param>
        public BlobReaderWriter(string dbpath1, string dbpath2)
        {
            // Form read-only connections to the database
            _errorCode = sqlite3_open_v2(dbpath1, ref _conn1, SQLITE_OPEN_READONLY, null);
            if (_errorCode > 0)
                throw new SQLiteException(_errorCode, "can't open file: " + dbpath1);
            _errorCode = sqlite3_open_v2(dbpath2, ref _conn2, SQLITE_OPEN_READONLY, null);
            if (_errorCode > 0)
                throw new SQLiteException(_errorCode, "can't open file: " + dbpath2);
        }

        ~BlobReaderWriter()
        {
            Dispose(false);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Write to the BLOB field specified by the table, column and row id. The BLOB will have
        /// a size of <paramref name="blobLength"/> and its contents will be fetched by calling the
        /// <paramref name="writer"/>delegate.
        /// </summary>
        /// <param name="tableName">The name of the table where the BLOB field exists.</param>
        /// <param name="columnName">The name of the column where the BLOB field exists.</param>
        /// <param name="rowId">The Row ID of the row where the BLOB field exists.</param>
        /// <param name="blobLength">The size of the BLOB to be written. This has to be known in advance!</param>
        /// <param name="writer">A delegate that will be called in order to fetch the bytes needed for writing
        /// into the BLOB field.</param>
        public void WriteBlob(string tableName, string columnName, long rowId, int blobLength, BlobWriter writer)
        {
            if (_disposed)
                throw new ObjectDisposedException("Object was disposed.");

            tableName = SQLiteParser.Utils.Chop(tableName);
            columnName = SQLiteParser.Utils.Chop(columnName);

            // Prepare an empty blob to which I can write the contents of the file.
            long len = blobLength;            

            // Open the BLOB handle and decide if the BLOB needs zeroing.
            _blob1 = IntPtr.Zero;
            bool needsZeroing = false;
            _errorCode = sqlite3_blob_open(_conn1, "main", tableName, columnName, rowId, 1, ref _blob1);
            if (_errorCode > 0)
            {
                _blob1 = IntPtr.Zero;
                needsZeroing = true;
            }
            else
            {
                // Query how big is that BLOB and decide if the BLOB needs zeroing or not.            
                int blobSize = sqlite3_blob_bytes(_blob1);
                if (blobSize != blobLength)
                    needsZeroing = true;
            } // else

            if (needsZeroing)
            {
                IntPtr stmt = IntPtr.Zero;
                IntPtr tail = IntPtr.Zero;

                // Close the BLOB handle if necessary
                if (_blob1 != IntPtr.Zero)
                {
                    _errorCode = sqlite3_blob_close(_blob1);
                    _blob1 = IntPtr.Zero;
                    if (_errorCode > 0)
                        throw new SQLiteException(_errorCode, "can't close BLOB handle");
                }

                // Prepare SQL statement for zeroing the BLOB field in the correct size for the BLOB
                // length we want to write to it.
                _errorCode = sqlite3_prepare_v2(_conn1, "UPDATE " + SQLiteParser.Utils.QuoteIfNeeded(tableName) + 
                    " SET " + SQLiteParser.Utils.QuoteIfNeeded(columnName) +
                    " = @blob WHERE RowID = @rowid", -1, ref stmt, ref tail);
                if (_errorCode > 0)
                    throw new SQLiteException(_errorCode, "sqlite3_prepare_v2 failed");

                try
                {
                    // Bind the BLOB parameter
                    _errorCode = sqlite3_bind_zeroblob(stmt, 1, (int)len);
                    if (_errorCode > 0)
                        throw new SQLiteException(_errorCode, "failed to bind zero-blob");

                    // Bind the RowID parameter
                    _errorCode = sqlite3_bind_int64(stmt, 2, rowId);
                    if (_errorCode > 0)
                        throw new SQLiteException(_errorCode, "failed to bind rowid");

                    // Execute the prepared statement
                    _errorCode = sqlite3_step(stmt);
                    if (_errorCode > 0 && _errorCode < 100)
                        throw new SQLiteException(_errorCode, "failed to execute zeroblob command");
                }
                finally
                {
                    // Finalize the prepared statement (we don't need it for more executions).
                    sqlite3_finalize(stmt);
                } // catch

                // Note: SQLite library won't let us open a BLOB with size zero so we simply return here.
                if (len == 0)
                {
                    _blob1 = IntPtr.Zero;
                    return;
                }

                // Open the BLOB handle
                _blob1 = IntPtr.Zero;
                _errorCode = sqlite3_blob_open(_conn1, "main", tableName, columnName, rowId, 1, ref _blob1);
                if (_errorCode > 0)
                    throw new SQLiteException(_errorCode, "sqlite3_blob_open failed");
            } // if

            // Write into the BLOB field by calling the blob writer delegate
            try
            {
                int offset = 0;
                int written = 0;
                while (written < len)
                {
                    int count = (int)(len - written);
                    if (count > PAGESIZE)
                        count = PAGESIZE;

                    // Call the writer delegate to fill my buffer
                    bool cancel = false;
                    int nbytes = writer(_buffer1, count, written, (int)len, ref cancel);
                    if (cancel)
                        throw new UserCancellationException();

                    // Write the new chunk to the BLOB
                    _errorCode = sqlite3_blob_write(_blob1, _buffer1, nbytes, offset);
                    if (_errorCode > 0)
                        throw new SQLiteException(_errorCode, "sqlite3_blob_write failed");

                    offset += nbytes;
                    written += nbytes;
                } // while
            }
            finally
            {
                // Close the BLOB handle
                sqlite3_blob_close(_blob1);

                // Reset the BLOB handle
                _blob1 = IntPtr.Zero;
            } // finally
        }

        /// <summary>
        /// Reads the entire BLOB field into memory
        /// </summary>
        /// <param name="tableName">The name of the table where the BLOB field is located</param>
        /// <param name="colName">The name of the column where the BLOB field is located</param>
        /// <param name="rowId">The ID of the row where the BLOB field is located</param>
        /// <returns>The BLOB data bytes</returns>
        public byte[] ReadBlob(string tableName, string colName, long rowId)
        {
            byte[] res = GetFieldBytes(_conn1, SQLiteParser.Utils.Chop(tableName), SQLiteParser.Utils.Chop(colName), rowId);
            return res;
        }

        /// <summary>
        /// Read the BLOB field in the specified table, column name and in row with the specified
        /// ROWID. The reading is performed in chunks that are passed to the provided reader delegate.
        /// </summary>
        /// <remarks>Never call this method when the value of the BLOB field is NULL - it won't work!</remarks>
        /// <param name="tableName">The name of the table where the BLOB field exists.</param>
        /// <param name="columnName">The name of the column where the BLOB field exists.</param>
        /// <param name="rowId">The ID of the row where the BLOB field exists.</param>
        /// <param name="reader">The BLOB reader delegate</param>
        public void ReadBlob(string tableName, string columnName, long rowId, BlobReader reader)
        {
            if (_disposed)
                throw new ObjectDisposedException("Object was disposed.");

            tableName = SQLiteParser.Utils.Chop(tableName);
            columnName = SQLiteParser.Utils.Chop(columnName);

            // Open the BLOB handle
            _errorCode = sqlite3_blob_open(_conn1, "main", tableName, columnName, rowId, 0, ref _blob1);
            if (_errorCode > 0)
            {
                // Sometimes - the database contains a non-BLOB object in a column that is declared a BLOB.
                // IN such cases - the call to sqlite3_blob_open will fail. When this happens we'll deal
                // with it by reading the contents of the field directly without using the BLOB API.

                IntPtr stmt = IntPtr.Zero;
                IntPtr tail = IntPtr.Zero;

                _errorCode = sqlite3_prepare_v2(_conn1, "SELECT "+
                    SQLiteParser.Utils.QuoteIfNeeded(columnName)+" FROM " + 
                    SQLiteParser.Utils.QuoteIfNeeded(tableName) + 
                    " WHERE RowID = @rowid", -1, ref stmt, ref tail);
                if (_errorCode > 0)
                    throw new SQLiteException(_errorCode, "failed to prepare SELECT statement");

                try
                {
                    // Bind the RowID parameter
                    _errorCode = sqlite3_bind_int64(stmt, 1, rowId);
                    if (_errorCode > 0)
                        throw new SQLiteException(_errorCode, "failed to bind rowid");

                    // Execute the prepared statement
                    _errorCode = sqlite3_step(stmt);
                    if (_errorCode > 0 && _errorCode < 100)
                        throw new SQLiteException(_errorCode, "failed to execute SELECT command");                    

                    string txt = sqlite3_column_text16(stmt, 0);
                    byte[] tmp = Encoding.ASCII.GetBytes(txt);

                    bool cancel = false;
                    reader(tmp, tmp.Length, 0, tmp.Length, ref cancel);
                    if (cancel)
                        throw new UserCancellationException();
                }
                finally
                {
                    // Finalize the prepared statement (we don't need it for more executions).
                    sqlite3_finalize(stmt);
                } // catch

                return;
            }

            try
            {
                // Query how big is that BLOB
                int count = sqlite3_blob_bytes(_blob1);
                int total = count;
                int offset = 0;
                while (count > 0)
                {
                    int toread = (count > PAGESIZE ? PAGESIZE : count);
                    _errorCode = sqlite3_blob_read(_blob1, _buffer1, toread, offset);
                    if (_errorCode > 0)
                        throw new SQLiteException(_errorCode, "sqlite3_blob_read failed");

                    bool cancel = false;
                    reader(_buffer1, toread, offset + toread, total, ref cancel);
                    if (cancel)
                        throw new UserCancellationException();

                    offset += toread;
                    count -= toread;
                } // while
            }
            finally
            {
                // Close the BLOB handle and reset it.
                sqlite3_blob_close(_blob1);

                // Reset the BLOB handle
                _blob1 = IntPtr.Zero;
            } // finally
        }

        /// <summary>
        /// Convenience method to write the BLOB field specified by the table name, column name and rowid
        /// to the file specified by the <paramref name="fpath"/> parameter.
        /// </summary>
        /// <param name="tableName">The table name where the BLOB exists</param>
        /// <param name="columnName">The column name where the BLOB exists.</param>
        /// <param name="rowId">The ROwID where the BLOB exists.</param>
        /// <param name="fpath">The path to the file that will contain the BLOB data.</param>
        /// <param name="reader">An optional delegate that will be called while reading the BLOB and can be used
        /// to notify about the progress of the write process.</param>
        public void ReadBlobToFile(string tableName, string columnName, long rowId, string fpath, BlobReader reader)
        {
            if (_disposed)
                throw new ObjectDisposedException("Object was disposed.");

            if (File.Exists(fpath))
                File.Delete(fpath);

            using (FileStream fs = File.Create(fpath, PAGESIZE))
            {
                bool first = true;

                // Use my own reader to get the BLOB chunks from the database and insert them into the file.
                BlobReader myreader = new BlobReader(delegate(byte[] buffer, int length, int bytesRead, int totalBytes, ref bool cancel)
                {
                    if (first)
                    {
                        // Set the overal size of the file before starting to write its contents.
                        fs.SetLength(totalBytes);
                        first = false;
                    }

                    // Write this chunk to the file.
                    fs.Write(buffer, 0, length);

                    // Call the actual delegate if necessary
                    if (reader != null)
                        reader(buffer, length, bytesRead, totalBytes, ref cancel);
                });

                ReadBlob(tableName, columnName, rowId, myreader);
            } // using
        }

        /// <summary>
        /// Write the BLOB data from the specified file <paramref name="fpath"/> to the BLOB field as 
        /// specified by the table name, column name and row id parameters. During the write-process - notify
        /// the optional <paramref name="writer"/> delegate about the progress.
        /// </summary>
        /// <param name="tableName">The table name where the BLOB exists</param>
        /// <param name="columnName">The column name where the BLOB exists.</param>
        /// <param name="rowId">The ROwID where the BLOB exists.</param>
        /// <param name="fpath">The path to the file that contains the BLOB data that should be written to the database.</param>
        /// <param name="writer">An optional writer delegate that will be informed throughout the process.</param>
        public void WriteBlobFromFile(string tableName, string columnName, long rowId, string fpath, BlobWriter writer)
        {
            if (_disposed)
                throw new ObjectDisposedException("Object was disposed.");

            using (FileStream fs = File.Open(fpath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                long len = fs.Length;

                // Ensure that we don't have too large files
                if (len > int.MaxValue)
                    throw new SQLiteException("The file is too large to be written as a BLOB");

                BlobWriter mywriter = new BlobWriter(delegate(byte[] buffer, int max, int written, int total, ref bool cancel)
                {
                    // Read the bytes from the file
                    int nbytes = fs.Read(buffer, 0, max);

                    // Call the actual delegate if necessary (but ignore its outputs).
                    if (writer != null)
                        writer(buffer, max, written, total, ref cancel);

                    // Return the number of bytes that were actually read from the file.
                    return nbytes;
                });

                WriteBlob(tableName, columnName, rowId, (int)len, mywriter);
            } // using
        }

        /// <summary>
        /// Copy a BLOB field from the source row to the target row
        /// </summary>
        /// <param name="targetdb">The path to the target row database</param>
        /// <param name="tableName">The name of the table where the row resides</param>
        /// <param name="columnName">The name of the BLOB column</param>
        /// <param name="fromRowId">The ID of the source row</param>
        /// <param name="toRowId">The ID of the target row</param>
        /// <param name="handler">An optional progress notifications handler</param>
        public void CopyBlob(string targetdb, string tableName, string columnName, long fromRowId, long toRowId, BlobProgressHandler handler)
        {
            if (_disposed)
                throw new ObjectDisposedException("Object was disposed.");

            if (_conn2 != IntPtr.Zero)
                throw new InvalidOperationException("You need to use a non-dual database constructor");

            // Open conncetion to the target database as read-write
            _errorCode = sqlite3_open(targetdb, ref _conn2);
            if (_errorCode > 0)
                throw new SQLiteException(_errorCode, "can't open file: " + targetdb);

            try
            {
                // Open source BLOB handle and compute its size
                _errorCode = sqlite3_blob_open(_conn1, "main", tableName, columnName, fromRowId, 0, ref _blob1);
                if (_errorCode > 0)
                    throw new SQLiteException(_errorCode, "failed to open BLOB handle");
                int count1 = sqlite3_blob_bytes(_blob1);

                bool needsResizing = false;

                // Open target BLOB handle and check if it needs to be resized first
                _errorCode = sqlite3_blob_open(_conn2, "main", tableName, columnName, toRowId, 1, ref _blob2);
                if (_errorCode > 0)
                    needsResizing = true;
                else
                {
                    int count2 = sqlite3_blob_bytes(_blob2);
                    if (count1 != count2)
                        needsResizing = true;
                } // else
                                
                // If the target BLOB needs resizing do it now.
                if (needsResizing)
                {
                    // We'll need to resize the target BLOB field
                    IntPtr stmt = IntPtr.Zero;
                    IntPtr tail = IntPtr.Zero;

                    // Close the BLOB handle if necessary
                    if (_blob2 != IntPtr.Zero)
                    {
                        _errorCode = sqlite3_blob_close(_blob2);
                        _blob2 = IntPtr.Zero;
                        if (_errorCode > 0)
                            throw new SQLiteException(_errorCode, "can't close BLOB handle");
                    }

                    // Prepare SQL statement for zeroing the BLOB field in the correct size for the BLOB
                    // length we want to write to it.
                    _errorCode = sqlite3_prepare_v2(_conn2, "UPDATE " + 
                        SQLiteParser.Utils.QuoteIfNeeded(tableName) + " SET " + 
                        SQLiteParser.Utils.QuoteIfNeeded(columnName) +
                        " = @blob WHERE RowID = @rowid", -1, ref stmt, ref tail);
                    if (_errorCode > 0)
                        throw new SQLiteException(_errorCode, "sqlite3_prepare_v2 failed");

                    try
                    {
                        // Bind the BLOB parameter
                        _errorCode = sqlite3_bind_zeroblob(stmt, 1, count1);
                        if (_errorCode > 0)
                            throw new SQLiteException(_errorCode, "failed to bind zero-blob");

                        // Bind the RowID parameter
                        _errorCode = sqlite3_bind_int64(stmt, 2, toRowId);
                        if (_errorCode > 0)
                            throw new SQLiteException(_errorCode, "failed to bind rowid");

                        // Execute the prepared statement
                        _errorCode = sqlite3_step(stmt);
                        if (_errorCode > 0 && _errorCode < 100)
                            throw new SQLiteException(_errorCode, "failed to execute zeroblob command");
                    }
                    finally
                    {
                        // Finalize the prepared statement (we don't need it for more executions).
                        sqlite3_finalize(stmt);
                    } // catch

                    // Re-open the target BLOB field
                    _errorCode = sqlite3_blob_open(_conn2, "main", tableName, columnName, toRowId, 1, ref _blob2);
                    if (_errorCode > 0)
                        throw new SQLiteException(_errorCode, "failed to re-open target BLOB handle");
                } // if

                int count = count1;
                int offset = 0;
                while (count > 0)
                {
                    int toread = (count > PAGESIZE ? PAGESIZE : count);

                    // Read the BLOB data from the source database
                    _errorCode = sqlite3_blob_read(_blob1, _buffer1, toread, offset);
                    if (_errorCode > 0)
                        throw new SQLiteException(_errorCode, "sqlite3_blob_read failed");

                    // Write it to the target database
                    _errorCode = sqlite3_blob_write(_blob2, _buffer1, toread, offset);
                    if (_errorCode > 0)
                        throw new SQLiteException(_errorCode, "sqlite3_blob_read failed");

                    offset += toread;
                    count -= toread;

                    if (handler != null)
                    {
                        bool cancel = false;
                        handler(offset, count1, ref cancel);
                        if (cancel)
                            throw new UserCancellationException();
                    }
                } // while

                // Close the blob handles
                sqlite3_blob_close(_blob1); _blob1 = IntPtr.Zero;
                sqlite3_blob_close(_blob2); _blob2 = IntPtr.Zero;

                // Close the target database connection
                sqlite3_close(_conn2); _conn2 = IntPtr.Zero;
            }
            finally
            {
                if (_blob1 != IntPtr.Zero)
                {
                    sqlite3_blob_close(_blob1);
                    _blob1 = IntPtr.Zero;
                }
                if (_blob2 != IntPtr.Zero)
                {
                    sqlite3_blob_close(_blob2);
                    _blob2 = IntPtr.Zero;
                }
                if (_conn2 != IntPtr.Zero)
                {
                    sqlite3_close(_conn2);
                    _conn2 = IntPtr.Zero;
                }
            } // finally
        }

        /// <summary>
        /// Compares two blob fields that reside in the two databases on which the blob reader writer object
        /// was opened (you need to use the dual databases constructor in order to use this method).
        /// </summary>
        /// <param name="tableName">The name of the table that contains the blob field</param>
        /// <param name="columnName">The name of the column that contains the blob field</param>
        /// <param name="rowid1">The row id in the first database where the blob field is located.</param>
        /// <param name="rowid2">The row id in the second database where the blob field is located.</param>
        /// <param name="comparer">An optional delegate to get progress notifications and be allowed to
        /// cancel the comparison process.</param>
        /// <returns>TRUE if the two blobs are the same, FALSE if they are different.</returns>
        public bool CompareBlobs(string tableName, string columnName, long rowid1, long rowid2, BlobProgressHandler comparer)
        {
            if (_disposed)
                throw new ObjectDisposedException("Object was disposed.");

            if (_conn2 == IntPtr.Zero)
                throw new InvalidOperationException("You need to use a dual database constructor");

            tableName = SQLiteParser.Utils.Chop(tableName);
            columnName = SQLiteParser.Utils.Chop(columnName);

            try
            {
                byte[] bbytes1 = null;
                byte[] bbytes2 = null;

                _errorCode = sqlite3_blob_open(_conn1, "main", tableName, columnName, rowid1, 0, ref _blob1);
                if (_errorCode > 0)
                {
                    bbytes1 = GetFieldBytes(_conn1, tableName, columnName, rowid1);
                }
                _errorCode = sqlite3_blob_open(_conn2, "main", tableName, columnName, rowid2, 0, ref _blob2);
                if (_errorCode > 0)
                {
                    bbytes2 = GetFieldBytes(_conn2, tableName, columnName, rowid2);
                }

                if (bbytes1 != null && bbytes2 != null)
                {
                    if (bbytes1.Length != bbytes2.Length)
                        return false;
                    for (int i = 0; i < bbytes1.Length; i++)
                    {
                        if (bbytes1[i] != bbytes2[i])
                            return false;
                    }
                    return true;
                }

                int count1;
                if (bbytes1 != null)
                    count1 = bbytes1.Length;
                else
                    count1 = sqlite3_blob_bytes(_blob1);

                int count2;
                if (bbytes2 != null)
                    count2 = bbytes2.Length;
                else
                    count2 = sqlite3_blob_bytes(_blob2);

                if (count1 != count2)
                    return false;

                int count = count1;

                int offset = 0;
                while (count > 0)
                {
                    int toread = (count > PAGESIZE ? PAGESIZE : count);

                    if (bbytes1 == null)
                    {
                        _errorCode = sqlite3_blob_read(_blob1, _buffer1, toread, offset);
                        if (_errorCode > 0)
                            throw new SQLiteException(_errorCode, "sqlite3_blob_read failed");
                    }

                    if (bbytes2 == null)
                    {
                        _errorCode = sqlite3_blob_read(_blob2, _buffer2, toread, offset);
                        if (_errorCode > 0)
                            throw new SQLiteException(_errorCode, "sqlite3_blob_read failed");
                    }

                    if (bbytes1 == null && bbytes2 == null)
                    {
                        for (int i = 0; i < toread; i++)
                        {
                            if (_buffer1[i] != _buffer2[i])
                                return false;
                        }
                    }
                    else if (bbytes1 != null)
                    {
                        for (int i = 0; i < bbytes1.Length; i++)
                        {
                            if (bbytes1[i] != _buffer2[i])
                                return false;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < bbytes2.Length; i++)
                        {
                            if (bbytes2[i] != _buffer1[i])
                                return false;
                        }
                    }

                    if (comparer != null)
                    {
                        bool cancel = false;
                        comparer(offset + toread, count1, ref cancel);
                        if (cancel)
                            throw new UserCancellationException();
                    }

                    offset += toread;
                    count -= toread;
                } // while

                // Close the blob handles
                if (bbytes1 == null)
                {
                    sqlite3_blob_close(_blob1); 
                    _blob1 = IntPtr.Zero;
                }

                if (bbytes2 == null)
                {
                    sqlite3_blob_close(_blob2); 
                    _blob2 = IntPtr.Zero;
                }

                return true;
            }
            finally
            {
                if (_blob1 != IntPtr.Zero)
                {
                    sqlite3_blob_close(_blob1);
                    _blob1 = IntPtr.Zero;
                }
                if (_blob2 != IntPtr.Zero)
                {
                    sqlite3_blob_close(_blob2);
                    _blob2 = IntPtr.Zero;
                }
            } // finally
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);     
        }

        #endregion

        #region Virtual Methods
        protected virtual void Dispose(bool disposing)
        {
            // If you need thread safety, use a lock around these 
            // operations, as well as in your methods that use the resource.
            if (!_disposed)
            {
                if (disposing)
                {
                    // There are no managed resources that can be disposed
                }

                // Close BLOB handle if necessary
                if (_blob1 != IntPtr.Zero)
                    sqlite3_blob_close(_blob1);

                // Close connection handle if necessary
                if (_conn1 != IntPtr.Zero)
                    sqlite3_close(_conn1);

                // Close BLOB handle if necessary
                if (_blob2 != IntPtr.Zero)
                    sqlite3_blob_close(_blob2);

                // Close connection handle if necessary
                if (_conn2 != IntPtr.Zero)
                    sqlite3_close(_conn2);

                // Indicate that the instance has been disposed.
                _conn1 = IntPtr.Zero;
                _conn2 = IntPtr.Zero;
                _blob1 = IntPtr.Zero;
                _blob2 = IntPtr.Zero;

                // Mark that the object was disposed
                _disposed = true;
            }
        }
        #endregion

        #region Private Methods
        private byte[] GetFieldBytes(IntPtr conn, string tableName, string columnName, long rowId)
        {
            IntPtr stmt = IntPtr.Zero;
            IntPtr tail = IntPtr.Zero;

            _errorCode = sqlite3_prepare_v2(conn, "SELECT " +
                SQLiteParser.Utils.QuoteIfNeeded(columnName) + " FROM " +
                SQLiteParser.Utils.QuoteIfNeeded(tableName) +
                " WHERE RowID = @rowid", -1, ref stmt, ref tail);
            if (_errorCode > 0)
                throw new SQLiteException(_errorCode, "failed to prepare SELECT statement");

            try
            {
                // Bind the RowID parameter
                _errorCode = sqlite3_bind_int64(stmt, 1, rowId);
                if (_errorCode > 0)
                    throw new SQLiteException(_errorCode, "failed to bind rowid");

                // Execute the prepared statement
                _errorCode = sqlite3_step(stmt);
                if (_errorCode > 0 && _errorCode < 100)
                    throw new SQLiteException(_errorCode, "failed to execute SELECT command");

                byte[] tmp = null;
                IntPtr bres = sqlite3_column_blob(stmt, 0);
                int sz = sqlite3_column_bytes16(stmt, 0);
                tmp = new byte[sz];
                Marshal.Copy(bres, tmp, 0, sz);

                return tmp;
            }
            finally
            {
                // Finalize the prepared statement (we don't need it for more executions).
                sqlite3_finalize(stmt);
            } // catch
        }
        #endregion

        #region Interop Methods
        /* int sqlite3_open(const char *filename, sqlite3 **ppDb); */
        [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_open(string filename, ref IntPtr ppDb);

        /* int sqlite3_open_v2(const char *filename, sqlite3 **ppDb,int flags,const char *zVfs); */
        [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_open_v2(string filename, ref IntPtr ppb, int flags, string zVfs);

        /* int sqlite3_close(sqlite3 *); */
        [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_close(IntPtr ppDb);

        [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_blob_open(IntPtr stmt, string zDb, string zTable, string zColumn, long iRow, int flags, ref IntPtr blobHandle);

        /* int sqlite3_blob_bytes(sqlite3_blob *); */
        [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_blob_bytes(IntPtr blobHandle);

        /* int sqlite3_blob_close(sqlite3_blob *); */
        [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_blob_close(IntPtr blobHandle);

        /* int sqlite3_blob_read(sqlite3_blob *, void *Z, int N, int iOffset); */
        [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_blob_read(IntPtr blobHandle, byte[] data, int len, int offset);

        /* int sqlite3_blob_write(sqlite3_blob *, const void *z, int n, int iOffset); */
        [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_blob_write(IntPtr blobHandle, byte[] data, int len, int offset);

        /* int sqlite3_prepare_v2(sqlite3 *db, const char *zSql, int nByte, sqlite3_stmt **ppStmt, const char **pzTail); */
        [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_prepare_v2(IntPtr ppDb, string sql, int maxLen, ref IntPtr stmt, ref IntPtr pzTail);

        /* int sqlite3_bind_zeroblob(sqlite3_stmt*, int, int n); */
        [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_bind_zeroblob(IntPtr stmt, int prmIndex, int size);

        /* int sqlite3_bind_int64(sqlite3_stmt*, int, sqlite3_int64); */
        [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_bind_int64(IntPtr stmt, int prmIndex, long value);

        /* int sqlite3_step(sqlite3_stmt*); */
        [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_step(IntPtr stmt);

        /* int sqlite3_finalize(sqlite3_stmt *pStmt); */
        [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_finalize(IntPtr stmt);

        /* int sqlite3_column_type(sqlite3_stmt*, int iCol); */
        [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_column_type(IntPtr stmt, int iCol);

        /* const void *sqlite3_column_text16(sqlite3_stmt*, int iCol); */
        [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern string sqlite3_column_text16(IntPtr stmt, int iCol);

        /* const void *sqlite3_column_blob(sqlite3_stmt*, int iCol);*/
        [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr sqlite3_column_blob(IntPtr stmt, int iCol);

        /* int sqlite3_column_bytes16(sqlite3_stmt*, int iCol); */
        [DllImport(SQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_column_bytes16(IntPtr stmt, int iCol);
        #endregion

        #region Private Constants
        private const int PAGESIZE = 4096;
        private const string SQLITE_DLL = "System.Data.SQLite.DLL";
        private const int SQLITE_OPEN_READONLY = 0x00000001;
        #endregion

        #region Private Variables
        private IntPtr _conn1 = IntPtr.Zero;
        private IntPtr _blob1 = IntPtr.Zero;
        private IntPtr _conn2 = IntPtr.Zero;
        private IntPtr _blob2 = IntPtr.Zero;
        private int _errorCode = 0;
        private bool _disposed = false;
        private byte[] _buffer1 = new byte[PAGESIZE];
        private byte[] _buffer2 = new byte[PAGESIZE];
        #endregion
    }

    /// <summary>
    /// A BLOB reader delegate that will be called during the BLOB reading process on every chunk of 
    /// data that is read from the BLOB field.
    /// </summary>
    /// <param name="buffer">The buffer that was read.</param>
    /// <param name="length">The length into that buffer that was actually read from the BLOB 
    /// (the buffer may be bigger than the amount of data that was actually read).</param>
    /// <param name="bytesRead">The total number of bytes that were read so far</param>
    /// <param name="totalBytes">The total number of bytes that are to read.</param>
    /// <param name="cancel">If the delegate wants to cancel the read process - it can set this parameter to
    /// TRUE. The reading process will stop by throwing a UserCancelledException exception.</param>
    public delegate void BlobReader(byte[] buffer, int length, int bytesRead, int totalBytes, ref bool cancel);

    /// <summary>
    /// A BLOB writer delegate that will be called during the BLOB writing process to fetch chunks of
    /// data that will be written to the BLOB field.
    /// </summary>
    /// <param name="buffer">The buffer that should be filled by the delegate</param>
    /// <param name="max">The maximum amount of bytes to write to the buffer.</param>
    /// <param name="written">The number of bytes that were already written to the BLOB</param>
    /// <param name="total">The total number of bytes that should be written to the BLOB</param>
    /// <param name="cancel">If the delegate will set this field to TRUE - the blob writing process will
    /// stop in the middle and the method will throw a UserCancelledException.</param>
    /// <returns>The total number of bytes that the method wrote to the buffer. Can be max or lower, but should be
    /// greater than 0</returns>
    public delegate int BlobWriter(byte[] buffer, int max, int written, int total, ref bool cancel);

    /// <summary>
    /// A BLOB comparer delegate that will be called during BLOB comparison for purpose of progress
    /// notification and allowing the delegate to cancel the operation.
    /// </summary>
    /// <param name="bytesRead">Tells how many bytes were read by the compare process</param>
    /// <param name="totalBytes">Tells the total number of bytes that need to be compared</param>
    /// <param name="cancel">If the delegate will set this field to TRUE - the blob comparison process
    /// will stop in the middle of the method and will throw a UserCancelledException.</param>
    public delegate void BlobProgressHandler(int bytesRead, int totalBytes, ref bool cancel);
    
}
