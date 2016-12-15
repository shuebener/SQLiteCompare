using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteFromIndexed
    {
        public SQLiteFromIndexed()
        {
        }

        public SQLiteFromIndexed(string by)
        {
            IndexedBy = by;
        }

        public bool IsIndexed
        {
            get { return _by != null; }
        }

        public string IndexedBy
        {
            get { return _by; }
            set
            {
                _by = Utils.QuoteIfNeeded(value);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteFromIndexed dst = obj as SQLiteFromIndexed;
            if (dst == null)
                return false;

            return _by == dst._by;
        }

        public override string ToString()
        {
            if (IsIndexed)
                return "INDEXED BY " + _by;
            else
                return "NOT INDEXED";
        }

        public virtual object Clone()
        {
            SQLiteFromIndexed res = new SQLiteFromIndexed();
            res._by = _by;
            return res;
        }

        private string _by;
    }
}
