using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteTurbo
{
    /// <summary>
    /// This class is responsible to copy rows from one table to another
    /// </summary>
    public class RowsCopier : AbstractWorker
    {
        #region Constructor
        public RowsCopier(TableChanges tchanges, string diff, List<TableChangesRange> rows, bool leftToRight)
            : base("RowsCopier")
        {
            _diff = diff;
            _changes = tchanges;
            _rows = rows;
            _leftToRight = leftToRight;
        }
        #endregion

        #region Protected Overrided Methods
        protected override void DoWork()
        {
            // Ask the table changes object to do the copy
            _changes.CopyRows(_diff, _rows, _leftToRight, RowsCopyingHandler);
        }

        protected override bool IsDualProgress
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region Private Methods
        private void RowsCopyingHandler(int rowsCopied, int totalRows, string blobName, int blobBytesCopied, int totalBlobBytes, ref bool cancel)
        {
            cancel = WasCancelled;

            if (blobName == null)
            {
                if (totalRows > 0)
                {
                    int progress = (int)(100.0 * rowsCopied / totalRows);
                    if (progress > _progress)
                    {
                        NotifyPrimaryProgress(false, progress, rowsCopied.ToString() + "/" + totalRows.ToString() + " copied", null);
                        _progress = progress;
                    }
                }
            }
            else
            {
                if (totalBlobBytes > 0)
                {
                    int progress = (int)(100.0 * blobBytesCopied / totalBlobBytes);
                    bool done = blobBytesCopied == totalBlobBytes;
                    NotifySecondaryProgress(done, progress, Utils.FormatMemSize(blobBytesCopied, MemFormat.KB) + "/" +
                        Utils.FormatMemSize(totalBlobBytes, MemFormat.KB) + " (" + blobName + ")", null);
                }
            } // else
        }
        #endregion

        #region Private Variables
        private string _diff;
        private int _progress;
        private TableChanges _changes;
        private List<TableChangesRange> _rows;
        private bool _leftToRight;
        #endregion
    }
}
