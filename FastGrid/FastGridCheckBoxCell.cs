using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;


namespace FastGridApp
{
    public class FastGridCheckBoxCell : FastGridCell
    {
        public FastGridCheckBoxCell(FastGrid owner)
            : base(owner)
        {
        }

        public override void OnPaint(Graphics g, Rectangle bbox, bool isRowSelected)
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

            if (Value != null)
            {
                Font f = GetFont();
                int fh = (int)Math.Ceiling(f.GetHeight(g));

                if (Value is bool)
                {
                    ControlPaint.DrawCheckBox(g, new Rectangle(bbox.X, bbox.Y + (bbox.Height - fh) / 2, bbox.Width, fh),
                        ((bool)Value) ? ButtonState.Checked : ButtonState.Normal);
                }
                else
                {
                    if (isRowSelected)
                    {
                        g.DrawString(Value.ToString(), GetFont(), SystemBrushes.HighlightText,
                            new Rectangle(bbox.X, bbox.Y + (bbox.Height - fh) / 2, bbox.Width, fh), _fmt);
                    }
                    else
                    {
                        using (SolidBrush b = new SolidBrush(GetForeColor()))
                            g.DrawString(Value.ToString(), GetFont(), b,
                                new Rectangle(bbox.X, bbox.Y + (bbox.Height - fh) / 2, bbox.Width, fh), _fmt);
                    }
                } // else
            }
        }

        private StringFormat _fmt = new StringFormat(StringFormatFlags.NoWrap);
    }
}
