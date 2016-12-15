using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FastGridApp
{
    public class FastGridColumn
    {
        public FastGridColumn(string text)
        {
            _text = text;
        }

        public FastGridColumn(Image img, string text)
        {
            _text = text;
            _img = img;
        }

        public FastGridColumn(string text, FastGridColumnType ctype)
        {
            _text = text;
            _ctype = ctype;
        }

        public FastGridColumn(Image img, string text, FastGridColumnType ctype)
        {
            _text = text;
            _ctype = ctype;
            _img = img;
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public Image Image
        {
            get { return _img; }
            set { _img = value; }
        }

        public FastGridColumnType ColumnType
        {
            get { return _ctype; }
            set { _ctype = value; }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        private object _tag;
        private string _text;
        private Image _img;
        private int _width = 100;
        private FastGridColumnType _ctype = FastGridColumnType.Text;
    }

    public enum FastGridColumnType
    {
        None = 0,

        Text = 1,

        CheckBox = 2,
    }
}
