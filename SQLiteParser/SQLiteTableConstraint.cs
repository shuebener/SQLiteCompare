using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteTableConstraint
    {
        private SQLiteTableConstraint()
        {
        }

        public SQLiteTableConstraint(string name)
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

        /// <summary>
        /// Used to merge CONSTRAINT xxx / NEXT constraint pairs (they are actually the
        /// same constraint).
        /// </summary>
        /// <param name="cons">The list to merge</param>
        /// <returns>The resulting (merged) constraints list</returns>
        public static List<SQLiteTableConstraint> Merge(List<SQLiteTableConstraint> cons)
        {
            List<SQLiteTableConstraint> res = new List<SQLiteTableConstraint>();
            for (int i = 0; i < cons.Count; i++)
            {
                SQLiteTableConstraint c = cons[i];
                if (c.GetType() == typeof(SQLiteTableConstraint) && c.ConstraintName != null)
                {
                    i++;
                    if (i < cons.Count)
                    {
                        SQLiteTableConstraint next = cons[i];
                        next.ConstraintName = c.ConstraintName;
                        res.Add(next);
                    }
                }
                else
                    res.Add(c);
            } // for

            return res;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteTableConstraint dst = obj as SQLiteTableConstraint;
            if (dst == null)
                return false;

            return this.ConstraintName == dst.ConstraintName;
        }

        public override string ToString()
        {
            return "<table constraint>";
        }

        public virtual object Clone()
        {
            SQLiteTableConstraint res = new SQLiteTableConstraint();
            res._name = _name;
            return res;
        }

        private string _name;
    }
}
