using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteObjectName
    {
        public SQLiteObjectName(string first, params string[] rest)
        {
            _first = Utils.QuoteIfNeeded(first);

            if (rest != null)
            {
                bool found = false;
                foreach (string r in rest)
                {
                    if (r != null)
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                    _rest = Utils.QuoteIfNeeded(rest);
            }
        }

        public string FirstName
        {
            get { return _first; }
        }

        public string[] Rest
        {
            get { return _rest; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(_first);
            if (_rest != null)
            {
                foreach (string r in _rest)
                    sb.Append("." + r);
            }
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteObjectName dst = obj as SQLiteObjectName;
            if (dst == null)
                return false;

            return dst.ToString().ToLower().Equals(this.ToString().ToLower());
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public virtual object Clone()
        {
            string[] r = null;
            if (_rest != null)
            {
                r = new string[_rest.Length];
                for(int i=0; i<_rest.Length; i++)
                    r[i] = _rest[i];
            }
            SQLiteObjectName res = new SQLiteObjectName(_first, r);
            return res;
        }

        private string _first;
        private string[] _rest;
    }
}
