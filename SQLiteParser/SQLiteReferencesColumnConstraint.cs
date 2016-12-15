using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteReferencesColumnConstraint : SQLiteColumnConstraint
    {
        private SQLiteReferencesColumnConstraint(string name)
            : base(name)
        {
        }

        public SQLiteReferencesColumnConstraint(string name, string tableName, List<SQLiteIndexedColumn> idxcols, List<SQLiteReferenceHandler> hlist)
            : base(name)
        {
            _columns = idxcols;
            _handlers = hlist;
            ForeignTable = tableName;
        }

        public string ForeignTable
        {
            get { return _foreignTable; }
            set 
            {
                _foreignTable = Utils.QuoteIfNeeded(value);
            }
        }

        public List<SQLiteIndexedColumn> Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        public List<SQLiteReferenceHandler> Handlers
        {
            get { return _handlers; }
            set { _handlers = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteReferencesColumnConstraint dst = obj as SQLiteReferencesColumnConstraint;
            if (dst == null)
                return false;

            if (_foreignTable != dst._foreignTable)
                return false;

            if (!RefCompare.CompareList<SQLiteIndexedColumn>(_columns, dst._columns))
                return false;
            if (!RefCompare.CompareList<SQLiteReferenceHandler>(_handlers, dst._handlers))
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (ConstraintName != null)
                sb.Append("CONSTRAINT " + ConstraintName + " ");
            sb.Append("REFERENCES " + _foreignTable);
            if (_columns != null && _columns.Count > 0)
            {
                sb.Append("(");
                for (int i = 0; i < _columns.Count; i++)
                {
                    sb.Append(_columns[i].ColumnName);
                    if (i < _columns.Count - 1)
                        sb.Append(",");
                } // for
                sb.Append(")");
            } // if

            if (_handlers != null)
            {
                sb.Append(" ");
                for (int i = 0; i < _handlers.Count; i++)
                {
                    sb.Append(_handlers[i].ToString());
                    if (i < _handlers.Count - 1)
                        sb.Append(" ");
                } // for
            }

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
            List<SQLiteReferenceHandler> handlers = null;
            if (_handlers != null)
            {
                handlers = new List<SQLiteReferenceHandler>();
                foreach (SQLiteReferenceHandler h in _handlers)
                    handlers.Add((SQLiteReferenceHandler)h.Clone());
            }

            SQLiteReferencesColumnConstraint res = new SQLiteReferencesColumnConstraint(ConstraintName);
            res._foreignTable = _foreignTable;
            res._columns = columns;
            res._handlers = handlers;
            return res;
        }

        private string _foreignTable;
        private List<SQLiteIndexedColumn> _columns;
        private List<SQLiteReferenceHandler> _handlers;
    }
}
