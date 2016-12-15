using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using FastGridApp;

namespace SQLiteTurbo
{
    /// <summary>
    /// This control displays a bar with hints where there are
    /// selected rows.
    /// </summary>
    public class SelectionBar : Control
    {
        #region Constructors
        public SelectionBar()
        {
            DoubleBuffered = true;
        }
        #endregion

        #region Public Properties
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public long CursorOffset
        {
            get { return _cursorOffset; }
            set { _cursorOffset = value; Invalidate(); }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the data source for painting the selection portion
        /// </summary>
        /// <param name="max">The maximum row id</param>
        /// <param name="selection">The selection ranges to paint</param>
        public void SetData(long max, FastGridSelection selection)
        {
            _max = max;
            _selection = selection;
            Invalidate();
        }
        #endregion

        #region Protected Overrided Methods
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            if (Width - 3 < 0)
                return;
            if (Height - 3 < 0)
                return;

            Rectangle border = new Rectangle(0, 0, Width - 1, Height - 1);
            using (Pen p = new Pen(this.ForeColor))
            {
                g.DrawRectangle(p, border);
            } // using

            Rectangle fill = new Rectangle(1, 1, Width - 3, Height - 3);
            using (SolidBrush b = new SolidBrush(this.BackColor))
            {
                g.FillRectangle(b, fill);
            } // using

            if (_max < 0 || _selection == null)
                return;

            // Draw selection ranges
            using (HatchBrush b = new HatchBrush(HatchStyle.ForwardDiagonal, SystemColors.Highlight, this.BackColor))
            {
                foreach (FastGridApp.SelectionRange range in _selection.SelectionRanges)
                {
                    int y1;
                    int y2;
                    if (range.StartRowId == 0 && range.EndRowId == 0 && _max == 0)
                    {
                        y1 = 1;
                        y2 = Height - 2;
                    }
                    else if (_max > 0)
                    {
                        y1 = TranslateOffsetToYPos(range.StartRowId);
                        y2 = TranslateOffsetToYPos(range.EndRowId);
                    }
                    else
                        continue;
                    g.FillRectangle(b, 1, y1, Width - 2, y2 - y1 + 1);
                } // foreach
            } // using

            // Draw the cursor line
            int ypos;
            if (_max == 0)
                ypos = 1;
            else
                ypos = TranslateOffsetToYPos(_cursorOffset);
            if (ypos < 0)
                return;
            using (Pen p = new Pen(this.ForeColor, 3))
            {
                p.StartCap = LineCap.ArrowAnchor;
                p.EndCap = LineCap.ArrowAnchor;
                g.DrawLine(p, 1, ypos, Width - 1, ypos);
            }
        }
        #endregion

        #region Private Methods
        private int TranslateOffsetToYPos(long offset)
        {
            int ypos = (int)(1.0 * offset / _max * (Height - 3)) + 1;
            return ypos;
        }
        #endregion

        #region Private Variables
        private long _max;
        private long _cursorOffset;
        private FastGridSelection _selection;
        #endregion
    }
}
