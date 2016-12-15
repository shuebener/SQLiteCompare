using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FastGridApp
{
    public class FastGridStyle
    {
        public Font Font
        {
            get { return _font; }
            set { _font = value; }
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

        private Font _font;
        private Color _backColor;
        private Color _foreColor;
    }
}
