using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace DiffControl
{
    /// <summary>
    /// A line record object contains the information for a single text line
    /// that appears in the diff edit box control.
    /// </summary>
    public class LineRecord : IEnumerable<TextRange>, IEnumerator<TextRange>, ICloneable
    {
        #region Constructors
        public LineRecord()
        {
            // Signifies a place holder line that doesn't actually contain any text
        }

        public LineRecord(int lineNumber, string text)
        {
            _text = text;
            if (_text != null)
                _lineNumber = lineNumber;
        }
        #endregion

        #region Public Methods
        public void Clear()
        {
            _ranges = null;
            _foreColor = Color.Empty;
            _backColor = Color.Empty;
        }

        public void AddRange(int start, int end, Color back, Color fore)
        {
            if (_text == null)
                throw new ArgumentException("it is illegal to add a text range for a place holder line");
            if (_ranges == null)
                _ranges = new List<TextRange>();
            if (start < 0 || start >= _text.Length || end < 0 || end > _text.Length ||
                end < start)
                throw new ArgumentException("illegal start/end indexes");
            _ranges.Add(new TextRange(start, end, back, fore));
        }

        public TextRange GetRangeForCharIndex(int index)
        {
            if (_ranges == null)
                return null;

            foreach (TextRange tr in _ranges)
            {
                if (index >= tr.StartIndex && index < tr.EndIndex)
                    return tr;
            } // foreach

            return null;
        }

        public TextRange this[int index]
        {
            get
            {
                if (_ranges == null)
                    throw new IndexOutOfRangeException();
                return _ranges[index];
            }

            set
            {
                if (_ranges == null)
                    throw new IndexOutOfRangeException();
                _ranges[index] = value;
            }
        }
        #endregion

        #region ICloneable Members

        public object Clone()
        {
            LineRecord res = new LineRecord();
            res._backColor = this._backColor;
            res._foreColor = this._foreColor;
            res._lineNumber = this._lineNumber;
            res._modified = this._modified;
            res._rangeIndex = this._rangeIndex;
            if (this._ranges != null)
            {
                res._ranges = new List<TextRange>();
                foreach (TextRange tr in this._ranges)
                    res._ranges.Add((TextRange)tr.Clone());
            }
            else
                res._ranges = null;
            res._text = this._text;
            return res;
        }

        #endregion

        #region IEnumerable<TextRange> Members

        public IEnumerator<TextRange> GetEnumerator()
        {
            return this;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        #endregion

        #region IEnumerator<TextRange> Members

        public TextRange Current
        {
            get
            {
                if (_rangeIndex == -1)
                    throw new IndexOutOfRangeException();
                return _ranges[_rangeIndex];
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // No need to implement
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get 
            {
                if (_rangeIndex == -1)
                    throw new IndexOutOfRangeException();
                return _ranges[_rangeIndex];
            }
        }

        public bool MoveNext()
        {
            if (_ranges == null || _rangeIndex + 1 >= _ranges.Count)
                return false;
            _rangeIndex++;
            return true;
        }

        public void Reset()
        {
            _rangeIndex = -1;
        }

        #endregion

        #region Public Properties
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                _ranges = null;
                _rangeIndex = -1;
            }
        }

        public bool IsModified
        {
            get { return _modified; }
            set { _modified = value; }
        }

        public int LineNumber
        {
            get { return _lineNumber; }
            set { _lineNumber = value; }
        }

        public Color BackColor
        {
            get { return _backColor; }
            set { _backColor = value; }
        }

        public Color ForeColor
        {
            get { return _foreColor; }
            set { _foreColor = value; }
        }

        public int RangesCount
        {
            get
            {
                if (_ranges == null)
                    return 0;
                else
                    return _ranges.Count;
            }
        }
        #endregion

        #region Private Variables
        private string _text;
        private int _lineNumber = -1;
        private Color _backColor;
        private Color _foreColor;
        List<TextRange> _ranges;
        private int _rangeIndex = -1;
        private bool _modified;
        #endregion
    }

    public class TextRange : ICloneable
    {
        public TextRange(int start, int end, Color back, Color fore)
        {
            _start = start;
            _end = end;
            _backColor = back;
            _foreColor = fore;
        }

        #region ICloneable Members

        public object Clone()
        {
            TextRange res = new TextRange(_start, _end, _backColor, _foreColor);
            return res;
        }

        #endregion

        #region Public Properties
        public int StartIndex
        {
            get { return _start; }
            set { _start = value; }
        }

        public int EndIndex
        {
            get { return _end; }
            set { _end = value; }
        }

        public Color BackColor
        {
            get { return _backColor; }
            set { _backColor = value; }
        }

        public Color ForeColor
        {
            get { return _foreColor; }
            set { _foreColor = value; }
        }
        #endregion

        #region Private Variables
        private int _start;
        private int _end;
        private Color _backColor;
        private Color _foreColor;
        #endregion
    }
}
