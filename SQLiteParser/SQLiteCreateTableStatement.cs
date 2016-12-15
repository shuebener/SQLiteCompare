using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteCreateTableStatement : SQLiteDdlStatement
    {
        private SQLiteCreateTableStatement(SQLiteObjectName tableName)
            : base(tableName)
        {
        }

        public SQLiteCreateTableStatement(bool ifNotExists, SQLiteObjectName tableName)
            : base(tableName)
        {
            _ifNotExists = ifNotExists;
        }

        public SQLiteCreateTableStatement(List<SQLiteColumnStatement> columns, List<SQLiteTableConstraint> constraints)
        {
            _columns = columns;
            if (constraints != null)
                _constraints = SQLiteTableConstraint.Merge(constraints);
            else
                _constraints = null;
        }

        public bool IfNotExists
        {
            get { return _ifNotExists; }
            set { _ifNotExists = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteCreateTableStatement dst = obj as SQLiteCreateTableStatement;
            if (dst == null)
                return false;

            if (dst.IfNotExists != this.IfNotExists)
                return false;

            if (!RefCompare.CompareList<SQLiteColumnStatement>(_columns, dst._columns))
                return false;
            if (!RefCompare.CompareList<SQLiteTableConstraint>(_constraints, dst._constraints))
                return false;

            return base.Equals(obj);
        }

        public string ToStatement(string tblName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("CREATE TABLE");
            if (_ifNotExists)
                sb.Append(" IF NOT EXISTS");
            tblName = Utils.QuoteIfNeeded(tblName);
            sb.Append(" " + tblName);
            if (_columns != null)
            {
                sb.Append("\r\n(\r\n");
                for (int i = 0; i < _columns.Count; i++)
                {
                    sb.Append("    " + _columns[i].ToString());
                    if (i < _columns.Count - 1)
                        sb.Append(",\r\n");
                } // for                
            }
            if (_constraints != null && _constraints.Count > 0)
            {
                sb.Append(",\r\n");
                for (int i = 0; i < _constraints.Count; i++)
                {
                    sb.Append("    " + _constraints[i].ToString());
                    if (i < _constraints.Count - 1)
                        sb.Append(",\r\n");
                } // for
            }
            sb.Append("\r\n)");

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToStatement(this.ObjectName.ToString());
        }

        public List<SQLiteColumnStatement> Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        public List<SQLiteTableConstraint> Constraints
        {
            get { return _constraints; }
            set { _constraints = value; }
        }

        public override object Clone()
        {
            List<SQLiteColumnStatement> columns = null;
            if (_columns != null)
            {
                columns = new List<SQLiteColumnStatement>();
                foreach (SQLiteColumnStatement cs in _columns)
                    columns.Add((SQLiteColumnStatement)cs.Clone());
            }

            List<SQLiteTableConstraint> constraints = null;
            if (_constraints != null)
            {
                constraints = new List<SQLiteTableConstraint>();
                foreach (SQLiteTableConstraint tc in _constraints)
                    constraints.Add((SQLiteTableConstraint)tc.Clone());
            }

            SQLiteObjectName tableName = null;
            if (this.ObjectName != null)
                tableName = (SQLiteObjectName)this.ObjectName.Clone();

            SQLiteCreateTableStatement res = new SQLiteCreateTableStatement(tableName);
            res._columns = columns;
            res._constraints = constraints;
            res._ifNotExists = _ifNotExists;
            return res;
        }

        private bool _ifNotExists;
        private List<SQLiteColumnStatement> _columns = new List<SQLiteColumnStatement>();
        private List<SQLiteTableConstraint> _constraints = new List<SQLiteTableConstraint>();
    }
}
