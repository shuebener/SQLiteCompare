using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteCaseItem
    {
        private SQLiteCaseItem()
        {
        }

        public SQLiteCaseItem(SQLiteExpression whenExpr, SQLiteExpression thenExpr)
        {
            _when = whenExpr;
            _then = thenExpr;
        }

        public SQLiteExpression WhenExpression
        {
            get { return _when; }
            set { _when = value; }
        }

        public SQLiteExpression ThenExpression
        {
            get { return _then; }
            set { _then = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteCaseItem dst = obj as SQLiteCaseItem;
            if (dst == null)
                return false;

            if (!RefCompare.CompareMany(_when, dst._when, _then, dst._then))
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            return "WHEN " + _when.ToString() + " THEN " + _then.ToString();
        }

        public virtual object Clone()
        {
            SQLiteExpression when = null;
            if (_when != null)
                when = (SQLiteExpression)_when.Clone();
            SQLiteExpression then = null;
            if (_then != null)
                then = (SQLiteExpression)_then.Clone();

            SQLiteCaseItem res = new SQLiteCaseItem();
            res._when = when;
            res._then = then;
            return res;
        }

        private SQLiteExpression _when;
        private SQLiteExpression _then;
    }
}
