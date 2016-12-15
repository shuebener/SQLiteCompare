using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteNullColumnConstraint : SQLiteColumnConstraint
    {
        private SQLiteNullColumnConstraint(string name)
            : base(name)
        {
        }

        public SQLiteNullColumnConstraint(string name, bool isnull, SQLiteResolveAction conf)
            : base(name)
        {
            _isnull = isnull;
            _conf = conf;
        }

        public bool IsNull
        {
            get { return _isnull; }
            set { _isnull = value; }
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

            SQLiteNullColumnConstraint dst = obj as SQLiteNullColumnConstraint;
            if (dst == null)
                return false;

            if (dst._isnull != this._isnull)
                return false;
            if (this._conf != dst._conf)
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (ConstraintName != null)
                sb.Append("CONSTRAINT " + ConstraintName + " ");
            if (_isnull)
                sb.Append("NULL");
            else
                sb.Append("NOT NULL");
            if (_conf != SQLiteResolveAction.None)
                sb.Append(" " + Utils.GetConflictClauseString(_conf));
            return sb.ToString();
        }

        public override object Clone()
        {
            SQLiteNullColumnConstraint res = new SQLiteNullColumnConstraint(this.ConstraintName);
            res._isnull = _isnull;
            res._conf = _conf;
            return res;
        }

        private bool _isnull;
        private SQLiteResolveAction _conf;
    }
}
