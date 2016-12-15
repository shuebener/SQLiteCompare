using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    class SQLiteCollateExpression : SQLiteExpression
    {
        private SQLiteCollateExpression()
        {
        }

        public SQLiteCollateExpression(SQLiteExpression expr, string collationName)
        {
            _expr = expr;
            CollationName = collationName;
        }

        public SQLiteExpression Expression
        {
            get { return _expr; }
            set { _expr = value; }
        }

        public string CollationName
        {
            get { return _collationName; }
            set 
            {
                _collationName = Utils.QuoteIfNeeded(value);
            }
        }

        public override bool IsConstant(bool allowNull)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteCollateExpression dst = obj as SQLiteCollateExpression;
            if (dst == null)
                return false;

            if (!RefCompare.Compare(_expr, dst._expr))
                return false;

            if (_collationName != dst._collationName)
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            string res = _expr.ToString() + " COLLATE " + _collationName;
            return res;
        }

        public override object Clone()
        {
            SQLiteExpression expr = null;
            if (_expr != null)
                expr = (SQLiteExpression)_expr.Clone();

            SQLiteCollateExpression res = new SQLiteCollateExpression();
            res._expr = expr;
            res._collationName = _collationName;
            return res;
        }

        private SQLiteExpression _expr;
        private string _collationName;
    }
}
