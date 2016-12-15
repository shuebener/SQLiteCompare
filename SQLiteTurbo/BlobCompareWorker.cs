using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteTurbo
{
    /// <summary>
    /// This class performs BLOB comparison
    /// </summary>
    public class BlobCompareWorker : AbstractWorker, IDisposable
    {
        #region Constructors & Destructors
        public BlobCompareWorker(string dbpath1, string dbpath2, string tableName, string columnName, long rowId1, long rowId2)
            : base("BlobCompareWorker")
        {
            _engine = new BlobReaderWriter(dbpath1, dbpath2);
            _tableName = tableName;
            _columnName = columnName;
            _rowId1 = rowId1;
            _rowId2 = rowId2;
        }

        ~BlobCompareWorker()
        {
            Dispose(false);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Get TRUE if the comparison result is that the two BLOBs are equal,
        /// Get FALSE if the comparison result is that the two BLOBs are different.
        /// </summary>
        public bool IsBlobsEqual
        {
            get { return _equalBlobs; }
        }
        #endregion

        #region Protected Overrided Methods
        protected override void DoWork()
        {
            // Call the BLOB reader object to do the job
            _equalBlobs = _engine.CompareBlobs(_tableName, _columnName, _rowId1, _rowId2, HandleBlobCompare);
        }

        protected override bool IsDualProgress
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected Virtual Methods
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Release managed resources
                    _engine.Dispose();
                }

                // Mark that the object was disposed
                _disposed = true;
            }
        }
        #endregion

        #region Private Methods
        private void HandleBlobCompare(int bytesRead, int totalBytes, ref bool cancel)
        {
            // Allow the user to cancel during comparison
            cancel = WasCancelled;

            // Send a progress notification
            int progress = 100;
            if (totalBytes > 0)
                progress = (int)(100.0*bytesRead / totalBytes);
            NotifyPrimaryProgress(false, progress, Utils.FormatMemSize(bytesRead, MemFormat.KB) + "/" +
                Utils.FormatMemSize(totalBytes, MemFormat.KB)+" compared so far", null);
        }
        #endregion

        #region Private Variables
        private BlobReaderWriter _engine;
        private string _tableName;
        private string _columnName;
        private long _rowId1;
        private long _rowId2;
        private bool _disposed;
        private bool _equalBlobs = false;
        #endregion
    }
}
