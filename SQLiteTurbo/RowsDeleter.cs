using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteTurbo
{
    /// <summary>
    /// This class is responsible to delete rows from the specified table
    /// </summary>
    public class RowsDeleter : AbstractWorker
    {
        #region Constructor
        public RowsDeleter(TableChanges tchanges, string diff, List<TableChangesRange> rows, bool left)
            : base("RowsDeleter")
        {
            _diff = diff;
            _changes = tchanges;
            _rows = rows;
            _left = left;
        }
        #endregion

        #region Protected Overrided Methods
        protected override void DoWork()
        {
            // Ask the table changes object to do the deletion
            _changes.DeleteRows(_diff, _rows, _left, RowsDeletionHandler);
        }

        protected override bool IsDualProgress
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region Private Methods
        private void RowsDeletionHandler(int rowsDeleted, int totalRows, ref bool cancel)
        {
            cancel = WasCancelled;
            if (totalRows > 0)
            {
                int progress = (int)(100.0 * rowsDeleted / totalRows);
                if (progress > _progress)
                {
                    NotifyPrimaryProgress(false, progress, rowsDeleted.ToString() + "/" + totalRows.ToString() + " deleted", null);
                    _progress = progress;
                }
            }
        }
        #endregion

        #region Private Variables
        private string _diff;
        private int _progress;
        private TableChanges _changes;
        private List<TableChangesRange> _rows;
        private bool _left;
        #endregion
    }
}
