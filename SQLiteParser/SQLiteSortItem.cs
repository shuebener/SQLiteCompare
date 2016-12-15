using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteSortItem
    {
        private SQLiteSortItem()
        {
        }

        public SQLiteSortItem(SQLiteExpression expr, SQLiteSortOrder order)
        {
            _expr = expr;
            _order = order;
        }

        public SQLiteExpression Expression
        {
            get { return _expr; }
            set { _expr = value; }
        }

        public SQLiteSortOrder SortOrder
        {
            get { return _order; }
            set { _order = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteSortItem dst = obj as SQLiteSortItem;
            if (dst == null)
                return false;

            if (!RefCompare.Compare(_expr, dst._expr))
                return false;

            if (_order != dst._order)
                return false;

            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(_expr.ToString());
            if (_order == SQLiteSortOrder.Ascending)
                sb.Append(" ASC");
            else if (_order == SQLiteSortOrder.Descending)
                sb.Append(" DESC");

            return sb.ToString();
        }

        public virtual object Clone()
        {
            SQLiteExpression expr = null;
            if (_expr != null)
                expr = (SQLiteExpression)_expr.Clone();

            SQLiteSortItem res = new SQLiteSortItem();
            res._expr = expr;
            res._order = _order;
            return res;
        }

        private SQLiteExpression _expr;
        private SQLiteSortOrder _order;
    }
}
