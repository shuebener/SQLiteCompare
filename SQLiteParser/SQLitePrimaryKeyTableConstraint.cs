using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLitePrimaryKeyTableConstraint : SQLiteTableConstraint
    {
        private SQLitePrimaryKeyTableConstraint(string name)
            : base(name)
        {
        }

        public SQLitePrimaryKeyTableConstraint(string name, List<SQLiteIndexedColumn> columns, bool autoincrement, SQLiteResolveAction conf)
            : base(name)
        {
            _columns = columns;
            _autoincrement = autoincrement;
            _conf = conf;
        }

        public bool IsAutoincrement
        {
            get { return _autoincrement; }
            set { _autoincrement = value; }
        }

        public SQLiteResolveAction ResolveAction
        {
            get { return _conf; }
            set { _conf = value; }
        }

        public List<SQLiteIndexedColumn> Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLitePrimaryKeyTableConstraint dst = obj as SQLitePrimaryKeyTableConstraint;
            if (dst == null)
                return false;

            if (this._autoincrement != dst._autoincrement)
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
            sb.Append("PRIMARY KEY (");
            for (int i = 0; i < _columns.Count; i++)
            {
                sb.Append(_columns[i].ToString());
                if (i < _columns.Count - 1)
                    sb.Append(",");
            } // for
            if (_autoincrement)
                sb.Append(" AUTOINCREMENT");
            sb.Append(")");
            if (_conf != SQLiteResolveAction.None)
                sb.Append(" "+Utils.GetConflictClauseString(_conf));

            return sb.ToString();
        }

        public override object Clone()
        {
            List<SQLiteIndexedColumn> columns = null;
            if (_columns != null)
            {
                columns = new List<SQLiteIndexedColumn>();
                foreach (SQLiteIndexedColumn c in _columns)
                {
                    columns.Add((SQLiteIndexedColumn)c.Clone());
                } // foreach
            }

            SQLitePrimaryKeyTableConstraint res = new SQLitePrimaryKeyTableConstraint(this.ConstraintName);
            res._autoincrement = _autoincrement;
            res._conf = _conf;
            res._columns = columns;
            return res;
        }

        private bool _autoincrement;
        private SQLiteResolveAction _conf;
        private List<SQLiteIndexedColumn> _columns;
    }
}
