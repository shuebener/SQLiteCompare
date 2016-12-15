using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteUniqueColumnConstraint : SQLiteColumnConstraint
    {
        private SQLiteUniqueColumnConstraint(string name)
            : base(name)
        {
        }

        public SQLiteUniqueColumnConstraint(string name, SQLiteResolveAction conf)
            : base(name)
        {
            _conf = conf;
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

            SQLiteUniqueColumnConstraint dst = obj as SQLiteUniqueColumnConstraint;
            if (dst == null)
                return false;

            if (_conf != dst._conf)
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (this.ConstraintName != null)
                sb.Append("CONSTRAINT " + this.ConstraintName + " ");
            sb.Append("UNIQUE");
            if (_conf != SQLiteResolveAction.None)
                sb.Append(" " + Utils.GetConflictClauseString(_conf));
            return sb.ToString();
        }

        public override object Clone()
        {
            SQLiteUniqueColumnConstraint res = new SQLiteUniqueColumnConstraint(this.ConstraintName);
            res._conf = _conf;
            return res;
        }

        private SQLiteResolveAction _conf;
    }
}
