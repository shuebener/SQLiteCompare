using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace FastGridApp
{
    public class FastGridCell
    {
        public FastGridCell(FastGrid owner)
        {
            _owner = owner;
        }

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public FastGridStyle Style
        {
            get { return _style; }
        }

        public virtual void OnPaint(Graphics g, Rectangle bbox, bool isRowSelected)
        {
            if (isRowSelected)
            {
                g.FillRectangle(SystemBrushes.Highlight, bbox);
            }
            else
            {
                using (SolidBrush b = new SolidBrush(GetBackColor()))
                    g.FillRectangle(b, bbox);
            }

            if (_value != null)
            {
                Font f = GetFont();
                int fh = (int)Math.Ceiling(f.GetHeight(g));

                if (isRowSelected)
                {
                    g.DrawString(_value.ToString(), GetFont(), SystemBrushes.HighlightText, 
                        new Rectangle(bbox.X, bbox.Y + (bbox.Height - fh) / 2, bbox.Width, fh), _fmt);
                }
                else
                {
                    using (SolidBrush b = new SolidBrush(GetForeColor()))
                        g.DrawString(_value.ToString(), GetFont(), b, 
                            new Rectangle(bbox.X, bbox.Y + (bbox.Height - fh) / 2, bbox.Width, fh), _fmt);
                }
            }
        }

        protected Color GetForeColor()
        {
            if (_style.ForeColor == Color.Empty)
                return _owner.ForeColor;
            else
                return _style.ForeColor;
        }

        protected Color GetBackColor()
        {
            if (_style.BackColor == Color.Empty)
                return _owner.BackColor;
            else
                return _style.BackColor;
        }

        protected Font GetFont()
        {
            if (_style.Font == null)
                return _owner.Font;
            else
                return _style.Font;
        }

        private object _value;
        private FastGrid _owner;
        private FastGridStyle _style = new FastGridStyle();
        private StringFormat _fmt = new StringFormat(StringFormatFlags.NoWrap);
    }
}
