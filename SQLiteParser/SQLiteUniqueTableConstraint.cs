using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteUniqueTableConstraint : SQLiteTableConstraint
    {
        private SQLiteUniqueTableConstraint(string name)
            : base(name)
        {
        }

        public SQLiteUniqueTableConstraint(string name, List<SQLiteIndexedColumn> columns, SQLiteResolveAction conf)
            : base(name)
        {
            _columns = columns;
            _conf = conf;
        }

        public SQLiteResolveAction ResolveAction
        {
            get { return _conf; }
            set { _conf = value; }
        }

        public List<SQLiteIndexedColumn> UniqueColumns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteUniqueTableConstraint dst = obj as SQLiteUniqueTableConstraint;
            if (dst == null)
                return false;

            if (this._conf != dst._conf)
                return false;
            if (!RefCompare.CompareList<SQLiteIndexedColumn>(_columns, dst._columns))
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (ConstraintName != null)
                sb.Append("CONSTRAINT " + ConstraintName+" ");
            sb.Append("UNIQUE (");
            for (int i = 0; i < _columns.Count; i++)
            {
                sb.Append(_columns[i].ToString());
                if (i < _columns.Count - 1)
                    sb.Append(",");
            } // for
            sb.Append(")");
            if (_conf != SQLiteResolveAction.None)
                sb.Append(" " + Utils.GetConflictClauseString(_conf));

            return sb.ToString();                       
        }

        public override object Clone()
        {
            List<SQLiteIndexedColumn> columns = null;
            if (_columns != null)
            {
                columns = new List<SQLiteIndexedColumn>();
                foreach (SQLiteIndexedColumn c in _columns)
                    columns.Add((SQLiteIndexedColumn)c.Clone());
            }

            SQLiteUniqueTableConstraint res = new SQLiteUniqueTableConstraint(this.ConstraintName);
            res._conf = _conf;
            res._columns = columns;
            return res;
        }

        private SQLiteResolveAction _conf;
        private List<SQLiteIndexedColumn> _columns;
    }
}
