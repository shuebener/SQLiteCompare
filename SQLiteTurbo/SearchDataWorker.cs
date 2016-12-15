using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteTurbo
{
    /// <summary>
    /// This class is responsible to search for a data row starting from the specified
    /// row index that matches the specified criteria
    /// </summary>
    public class SearchDataWorker : AbstractWorker
    {
        #region Constructors
        public SearchDataWorker(bool isLeft, string diff, TableChanges tchanges, long rowIndex, long maxRowIndex, string sql)
            : base("SearchDataWorker")
        {
            _maxRowIndex = maxRowIndex;
            _isLeft = isLeft;
            _changes = tchanges;
            _rowIndex = rowIndex;
            _sql = sql;
            _diff = diff;
        }
        #endregion

        #region Public Properties
        #endregion

        #region Protected Methods
        protected override void DoWork()
        {
            if (_sql != null)
                WorkResult = _changes.SearchRowsBySQL(_isLeft, _diff, _rowIndex, _maxRowIndex, _sql, SearchHandler);
        }

        protected override bool IsDualProgress
        {
            get { return false;  }
        }        
        #endregion

        #region Private Methods
        private void SearchHandler(long searchedRows, long total, ref bool cancel)
        {
            cancel = WasCancelled;
            if (total == 0)
                return;
            int next = (int)(100.0 * searchedRows / total);
            if (next > _progress)
            {
                _progress = next;
                NotifyPrimaryProgress(false, _progress, searchedRows.ToString() + "/" + total.ToString() + " rows searched", null);
            }
        }
        #endregion

        #region Private Variables
        private bool _isLeft;
        private string _diff;
        private TableChanges _changes;
        private long _rowIndex;
        private long _maxRowIndex = -1;
        private string _sql;
        private int _progress;
        #endregion
    }
}
