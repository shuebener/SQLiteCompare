using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteFromInternalTable
    {
        private SQLiteFromInternalTable()
        {
        }

        public SQLiteFromInternalTable(SQLiteSelectStatement select)
        {
            _select = select;
        }

        public SQLiteFromInternalTable(SQLiteFromClause from)
        {
            _from = from;
        }

        public SQLiteSelectStatement SelectStatement
        {
            get { return _select; }
            set { _select = value; }
        }

        public SQLiteFromClause FromClause
        {
            get { return _from; }
            set { _from = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteFromInternalTable dst = obj as SQLiteFromInternalTable;
            if (dst == null)
                return false;

            if (!RefCompare.CompareMany(_select, dst._select, _from, dst._from))
                return false;

            return true;
        }

        public override string ToString()
        {
            if (_select != null)
                return _select.ToString();
            else
                return _from.ToString();
        }

        public virtual object Clone()
        {
            SQLiteSelectStatement stmt = null;
            if (_select != null)
                stmt = (SQLiteSelectStatement)_select.Clone();
            SQLiteFromClause f = null;
            if (_from != null)
                f = (SQLiteFromClause)_from.Clone();
            SQLiteFromInternalTable res = new SQLiteFromInternalTable();
            res._from = f;
            res._select = stmt;
            return res;
        }

        private SQLiteSelectStatement _select;
        private SQLiteFromClause _from;
    }
}
