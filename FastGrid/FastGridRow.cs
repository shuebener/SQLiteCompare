using System;
using System.Collections.Generic;
using System.Text;

namespace FastGridApp
{
    public class FastGridRow
    {
        public FastGridRow(FastGrid owner)
        {
            _owner = owner;
        }

        public FastGridCell[] Cells
        {
            get { return _cells; }
            set { _cells = value; }
        }

        public FastGridStyle Style
        {
            get { return _style; }
        }

        public object Tag
        {
            get { return _tag; }
        }

        private object _tag;
        private FastGrid _owner;
        private FastGridStyle _style = new FastGridStyle();
        private FastGridCell[] _cells;
    }
}
