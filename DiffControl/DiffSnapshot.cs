using System;
using System.Collections.Generic;
using System.Text;

namespace DiffControl
{
    /// <summary>
    /// Encapsulates the full state of the diff edit box (including lines, cursor, selection etc)
    /// so it can be restored later.
    /// </summary>
    public class DiffSnapshot : ICloneable
    {
        #region Constructors
        public DiffSnapshot(List<LineRecord> lines, DiffEditPosition cursor, 
            DiffEditRange selection, int lineIndex, int column,
            bool modified, bool synched)
        {
            if (lines == null)
                _lines = null;
            else
            {
                _lines = new List<LineRecord>();
                foreach (LineRecord lrec in lines)
                    _lines.Add((LineRecord)lrec.Clone());
            } // else
            _cursor = cursor;
            _selection = selection;
            _lineIndex = lineIndex;
            _columnIndex = column;
            _modified = modified;
            _synched = synched;
        }
        #endregion

        #region ICloneable Members

        public object Clone()
        {
            DiffSnapshot res = new DiffSnapshot(_lines, _cursor, _selection, _lineIndex, _columnIndex, _modified, _synched);
            return res;
        }

        #endregion

        #region Public Properties
        public List<LineRecord> Lines
        {
            get { return _lines; }
        }

        public DiffEditPosition Cursor
        {
            get { return _cursor; }
            set { _cursor = value; }
        }

        public DiffEditRange Selection
        {
            get { return _selection; }
            set { _selection = value; }
        }

        public int LineIndex
        {
            get { return _lineIndex; }
            set { _lineIndex = value; }
        }

        public int ColumnIndex
        {
            get { return _columnIndex; }
            set { _columnIndex = value; }
        }

        public bool IsModified
        {
            get { return _modified; }
            set { _modified = value; }
        }

        public bool IsSynched
        {
            get { return _synched; }
            set { _synched = value; }
        }
        #endregion

        #region Private Variables
        private List<LineRecord> _lines;
        private DiffEditPosition _cursor;
        private DiffEditRange _selection;
        private int _lineIndex;
        private int _columnIndex;
        private bool _modified;
        private bool _synched;
        #endregion
    }
}
