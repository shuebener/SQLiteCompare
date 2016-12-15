using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteCheckColumnConstraint : SQLiteColumnConstraint
    {
        private SQLiteCheckColumnConstraint(string name)
            : base(name)
        {
        }

        public SQLiteCheckColumnConstraint(string name, SQLiteExpression expr)
            : base(name)
        {
            _expr = expr;
        }

        public SQLiteExpression Expression
        {
            get { return _expr; }
            set { _expr = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteCheckColumnConstraint dst = obj as SQLiteCheckColumnConstraint;
            if (dst == null)
                return false;

            if (!this.Expression.Equals(dst.Expression))
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            if (this.ConstraintName != null)
                return "CONSTRAINT " + this.ConstraintName + " CHECK (" + _expr.ToString() + ")";
            else
                return "CHECK (" + _expr.ToString() + ")";
        }

        public override object Clone()
        {
            SQLiteExpression expr = null;
            if (_expr != null)
                expr = (SQLiteExpression)_expr.Clone();

            SQLiteCheckColumnConstraint res = new SQLiteCheckColumnConstraint(this.ConstraintName);
            res._expr = expr;
            return res;
        }

        private SQLiteExpression _expr;
    }
}
