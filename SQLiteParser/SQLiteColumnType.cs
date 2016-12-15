using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteColumnType
    {
        #region Constructors
        public SQLiteColumnType(string name, int size, int precision)
        {
            _name = Utils.QuoteIfNeeded(name);
            _size = size;
            _precision = precision;
        }
        #endregion

        #region Public Properties
        public string TypeName
        {
            get { return _name; }
            set { _name = Utils.QuoteIfNeeded(value); }
        }

        public int TypeSize
        {
            get { return _size; }
            set { _size = value; }
        }

        public int TypePrecision
        {
            get { return _precision; }
            set { _precision = value; }
        }
        #endregion

        #region Public Overrided Methods
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteColumnType dst = obj as SQLiteColumnType;
            if (dst == null)
                return false;

            if (_name != dst._name || _size != dst._size || _precision != dst._precision)
                return false;

            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(_name);
            if (_size > 0 && _precision > 0)
                sb.Append(string.Format("({0},{1})", _size, _precision));
            else if (_size > 0)
                sb.Append(string.Format("({0})", _size));
            return sb.ToString();
        }
        #endregion

        public virtual object Clone()
        {
            return new SQLiteColumnType(_name, _size, _precision);
        }

        #region Private Variables
        private string _name;
        private int _size;
        private int _precision;
        #endregion
    }
}
