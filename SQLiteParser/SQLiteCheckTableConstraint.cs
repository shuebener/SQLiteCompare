using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteCheckTableConstraint : SQLiteTableConstraint
    {
        private SQLiteCheckTableConstraint(string name)
            : base(name)
        {
        }

        public SQLiteCheckTableConstraint(string name, SQLiteExpression expr, SQLiteResolveAction conf)
            : base(name)
        {
            _expr = expr;
            _conf = conf;
        }

        public SQLiteExpression Expression
        {
            get { return _expr; }
            set { _expr = value; }
        }

        public SQLiteResolveAction ResolveAction
        {
            get { return _conf; }
            set { _conf = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteCheckTableConstraint dst = obj as SQLiteCheckTableConstraint;
            if (dst == null)
                return false;

            if (!RefCompare.CompareMany(_expr, dst._expr, _conf, dst._conf))
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (ConstraintName != null)
                sb.Append("CONSTRAINT " + ConstraintName+ " ");
            sb.Append("CHECK (" + _expr.ToString() + ")");
            if (_conf != SQLiteResolveAction.None)
                sb.Append(" " + Utils.GetConflictClauseString(_conf));
            return sb.ToString();
        }

        public override object Clone()
        {
            SQLiteExpression expr = null;
            if (_expr != null)
                expr = (SQLiteExpression)_expr.Clone();

            SQLiteCheckTableConstraint res = new SQLiteCheckTableConstraint(this.ConstraintName);
            res._expr = expr;
            res._conf = _conf;
            return res;
        }

        private SQLiteExpression _expr;
        private SQLiteResolveAction _conf;
    }
}
