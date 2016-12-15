using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteTurbo
{
    /// <summary>
    /// This class is responsible to copy BLOB fields from one database to another
    /// </summary>
    public class BlobCopier : AbstractWorker, IDisposable
    {
        #region Constructors & Destructors
        public BlobCopier(string sourcedb, string targetdb, string tableName, string columnName, long fromRowId, long toRowId)
            : base ("BlobCopier")
        {
            _engine = new BlobReaderWriter(sourcedb, true);
            _targetdb = targetdb;
            _tableName = tableName;
            _columnName = columnName;
            _fromRowId = fromRowId;
            _toRowId = toRowId;
        }

        ~BlobCopier()
        {
            Dispose(false);
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected Overrided Methods
        protected override void DoWork()
        {
            _engine.CopyBlob(_targetdb, _tableName, _columnName, _fromRowId, _toRowId, CopyProgressHandler);
        }

        protected override bool IsDualProgress
        {
            get
            {
                return false;
            }
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
        private void CopyProgressHandler(int bytesRead, int totalBytes, ref bool cancel)
        {
            cancel = WasCancelled;

            if (totalBytes > 0)
            {
                int progress = (int)(100.0 * bytesRead / totalBytes);
                if (progress > _progress)
                {
                    NotifyPrimaryProgress(false, progress,
                        Utils.FormatMemSize(bytesRead, MemFormat.KB) + "/" +
                        Utils.FormatMemSize(totalBytes, MemFormat.KB) + " copied so far", null);
                    _progress = progress;
                } // if
            } // if 
        }
        #endregion

        #region Private Variables
        private BlobReaderWriter _engine;
        private string _targetdb;
        private string _tableName;
        private string _columnName;
        private long _fromRowId;
        private long _toRowId;
        private bool _disposed;
        private int _progress;
        #endregion
    }
}
