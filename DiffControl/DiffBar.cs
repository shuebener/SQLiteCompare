using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DiffControl
{
    /// <summary>
    /// Provides a summary view of the differences that were found in
    /// the databases schema text.
    /// </summary>
    public class DiffBar : Control
    {
        #region Constructors
        public DiffBar()
        {
            DoubleBuffered = true;
        }
        #endregion

        #region Public Properties
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int RowsCount
        {
            get { return _rowsCount; }
            set 
            { 
                _rowsCount = value;
                DoLayout();
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
                DoLayout();
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PageStartRow
        {
            get { return _pageStartRow; }
            set
            {
                if (value > _rowsCount - _pageSize)
                    _pageStartRow = _rowsCount - _pageSize;
                else if (value < 0)
                    _pageStartRow = 0;
                else                    
                    _pageStartRow = value;

                DoLayout();
                Invalidate();
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Add a new colors column to the diff bar.
        /// </summary>
        /// <param name="columnColors">A dictionary that maps from a row index to the relevant
        /// color to draw in the diff bar.</param>
        public void AddColumn(Dictionary<int, Color> columnColors)
        {
            _columns.Add(columnColors);
            DoLayout();
            Invalidate();
        }

        /// <summary>
        /// Clear the columns from the diff bar
        /// </summary>
        public void Clear()
        {
            _columns.Clear();
            _rowsCount = 0;
            DoLayout();
            Invalidate();
        }

        /// <summary>
        /// Translates a point to the matching row index
        /// </summary>
        /// <param name="pt">The point to translate</param>
        /// <returns>The relevant row index</returns>
        public int GetRowIndexFromPoint(Point pt)
        {
            int row = (int)Math.Ceiling((pt.Y - _contentsRect.Y) / _rowHeight);
            if (row < 0)
                row = 0;
            if (row >= _rowsCount)
                row = _rowsCount - 1;

            return row;
        }
        #endregion

        #region Protected Overrided Methods

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            DoLayout();
            Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!_init)
            {
                DoLayout();
                _init = true;
            }

            // First draw the control's border rectangle
            Graphics g = e.Graphics;
            using (Pen p = new Pen(this.ForeColor))
                g.DrawRectangle(p, _borderRect);
            using (SolidBrush b = new SolidBrush(this.BackColor))
                g.FillRectangle(b, _contentsRect);

            if (!_valid)
                return;

            // Draw the columns
            float rh = (float)Math.Ceiling(_rowHeight);
            for (int i = 0; i < _columns.Count; i++)
            {
                Dictionary<int, Color> colors = _columns[i];
                foreach (int row in colors.Keys)
                {
                    RectangleF r = new RectangleF(_crects[i].X, _contentsRect.Y+_rowHeight * row, _crects[i].Width, rh);
                    using (SolidBrush b = new SolidBrush(colors[row]))
                        g.FillRectangle(b, r);
                } // foreach
            } // for

            if (_pageStartRow == 0 && _pageSize == _rowsCount)
            {
                // When the entire rows range is contained in the current page - don't
                // show the selection at all.
            }
            else
            {
                // Draw the page area
                using (SolidBrush b = new SolidBrush(PAGE_COLOR))
                    g.FillRectangle(b, _pageRect);
            }
        }
        #endregion

        #region Private Methods
        private void DoLayout()
        {
            _borderRect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            _contentsRect = new Rectangle(1, 1, this.Width - 2, this.Height - 2);

            if (_columns.Count == 0 || _rowsCount == 0)
            {
                _valid = false;
                return;
            }
            else
                _valid = true;

            // Compute the rectangles for each color column
            float colWidth = 1.0F * _contentsRect.Width / _columns.Count;
            _crects.Clear();
            for (int i = 0; i < _columns.Count; i++)
            {
                Rectangle r = new Rectangle(_contentsRect.X + (int)(i * colWidth), _contentsRect.Y, (int)colWidth, _contentsRect.Height);
                _crects.Add(r);
            } // for

            // Compute the height of each row in the column
            _rowHeight = 1.0F*_contentsRect.Height / _rowsCount;

            // Compute the page rectangle
            _pageRect = new RectangleF(_contentsRect.X, _contentsRect.Y + _rowHeight * _pageStartRow,
                _contentsRect.Width, (float)Math.Ceiling(_rowHeight * _pageSize));
        }
        #endregion

        #region Constants
        private Color PAGE_COLOR = Color.FromArgb(70, SystemColors.Highlight);
        #endregion

        #region Private Variables
        private bool _init;
        private bool _valid;
        private int _rowsCount;
        private int _pageSize;
        private int _pageStartRow;
        private float _rowHeight;
        private RectangleF _pageRect;
        private Rectangle _borderRect;
        private Rectangle _contentsRect;
        private List<Rectangle> _crects = new List<Rectangle>();
        private List<Dictionary<int, Color>> _columns = new List<Dictionary<int, Color>>();
        #endregion
    }
}
