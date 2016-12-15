using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteUpdateItem 
    {
        private SQLiteUpdateItem()
        {
        }

        public SQLiteUpdateItem(string colname, SQLiteExpression expr)
        {
            ColumnName = colname;
            _expr = expr;
        }

        public SQLiteExpression Expression
        {
            get { return _expr; }
            set { _expr = value; }
        }

        public string ColumnName
        {
            get { return _colname; }
            set 
            {
                _colname = Utils.QuoteIfNeeded(value);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteUpdateItem dst = obj as SQLiteUpdateItem;
            if (dst == null)
                return false;

            if (_colname != dst._colname)
                return false;
            if (!RefCompare.Compare(_expr, dst._expr))
                return false;

            return true;
        }

        public override string ToString()
        {
            return _colname + " = " + _expr.ToString();
        }

        public virtual object Clone()
        {
            SQLiteExpression expr = null;
            if (_expr != null)
                expr = (SQLiteExpression)_expr.Clone();

            SQLiteUpdateItem res = new SQLiteUpdateItem();
            res._colname = _colname;
            res._expr = expr;
            return res;
        }

        private string _colname;
        private SQLiteExpression _expr;
    }
}
