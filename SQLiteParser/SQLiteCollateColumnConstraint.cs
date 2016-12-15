using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteCollateColumnConstraint : SQLiteColumnConstraint
    {
        private SQLiteCollateColumnConstraint(string name)
            : base(name)
        {
        }

        public SQLiteCollateColumnConstraint(string name, string collation)
            : base(name)
        {
            CollationName = collation;
        }

        public string CollationName
        {
            get { return _collation; }
            set 
            {
                _collation = Utils.QuoteIfNeeded(value);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteCollateColumnConstraint dst = obj as SQLiteCollateColumnConstraint;
            if (dst == null)
                return false;

            if (this.CollationName != dst.CollationName)
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            if (ConstraintName != null)
                return "CONSTRAINT " + ConstraintName + " COLLATE " + _collation;
            else
                return "COLLATE " + _collation;
        }

        public override object Clone()
        {
            SQLiteCollateColumnConstraint res = new SQLiteCollateColumnConstraint(this.ConstraintName);
            res._collation = _collation;
            return res;
        }

        private string _collation;
    }
}
