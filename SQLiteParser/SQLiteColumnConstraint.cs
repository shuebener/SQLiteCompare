using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteColumnConstraint
    {
        public SQLiteColumnConstraint()
        {
        }

        public SQLiteColumnConstraint(string name)
        {
            ConstraintName = name;
        }

        public string ConstraintName
        {
            get { return _name; }
            set 
            {
                _name = Utils.QuoteIfNeeded(value);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteColumnConstraint dst = obj as SQLiteColumnConstraint;
            if (dst == null)
                return false;

            return _name == dst._name;
        }

        public virtual object Clone()
        {
            SQLiteColumnConstraint res = new SQLiteColumnConstraint();
            res._name = _name;
            return res;
        }

        private string _name;
    }
}
