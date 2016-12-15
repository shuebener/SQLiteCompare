using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteForeignKeyTableConstraint : SQLiteTableConstraint
    {
        private SQLiteForeignKeyTableConstraint(string name)
            : base(name)
        {
        }

        public SQLiteForeignKeyTableConstraint(string name, List<SQLiteIndexedColumn> columns, string foreignTable,
            List<SQLiteIndexedColumn> foreignColumns, List<SQLiteReferenceHandler> handlers,
            SQLiteDeferColumnConstraint deferConstraint)
            : base(name)
        {
            _columns = columns;
            ForeignTable = foreignTable;
            _foreignColumns = foreignColumns;
            _handlers = handlers;
            _deferConstraint = deferConstraint;
        }

        public List<SQLiteIndexedColumn> Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        public string ForeignTable
        {
            get { return _foreignTable; }
            set 
            {
                _foreignTable = Utils.QuoteIfNeeded(value);
            }
        }

        public List<SQLiteIndexedColumn> ForeignColumns
        {
            get { return _foreignColumns; }
            set { _foreignColumns = value; }
        }

        public List<SQLiteReferenceHandler> Handlers
        {
            get { return _handlers; }
            set { _handlers = value; }
        }

        public SQLiteDeferColumnConstraint DeferConstraint
        {
            get { return _deferConstraint; }
            set { _deferConstraint = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteForeignKeyTableConstraint dst = obj as SQLiteForeignKeyTableConstraint;
            if (dst == null)
                return false;

            if (!RefCompare.CompareList<SQLiteIndexedColumn>(_columns, dst._columns))
                return false;
            if (this._foreignTable != dst._foreignTable)
                return false;
            if (!RefCompare.CompareList<SQLiteIndexedColumn>(_foreignColumns, dst._foreignColumns))
                return false;
            if (!RefCompare.CompareList<SQLiteReferenceHandler>(_handlers, dst._handlers))
                return false;
            if (!RefCompare.Compare(_deferConstraint, dst._deferConstraint))
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (ConstraintName != null)
                sb.Append("CONSTRAINT " + ConstraintName+" ");
            sb.Append("FOREIGN KEY ");
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
            }
            sb.Append(" REFERENCES "+_foreignTable);
            if (_foreignColumns != null && _foreignColumns.Count > 0)
            {
                sb.Append("(");
                for (int i = 0; i < _foreignColumns.Count; i++)
                {
                    sb.Append(_foreignColumns[i].ColumnName);
                    if (i < _foreignColumns.Count - 1)
                        sb.Append(",");
                } // for
                sb.Append(")");
            }
            if (_handlers != null && _handlers.Count > 0)
            {
                for (int i = 0; i < _handlers.Count; i++)
                    sb.Append(" "+_handlers[i].ToString());
            }
            if (_deferConstraint != null)
                sb.Append(" "+_deferConstraint.ToString());

            return sb.ToString();
        }

        public override object Clone()
        {
            List<SQLiteIndexedColumn> columns = null;
            if (_columns != null)
            {
                columns = new List<SQLiteIndexedColumn>();
                foreach (SQLiteIndexedColumn ic in _columns)
                    columns.Add((SQLiteIndexedColumn)ic.Clone());
            }

            List<SQLiteIndexedColumn> foreignColumns = null;
            if (_foreignColumns != null)
            {
                foreignColumns = new List<SQLiteIndexedColumn>();
                foreach (SQLiteIndexedColumn ic in _foreignColumns)
                    foreignColumns.Add((SQLiteIndexedColumn)ic.Clone());
            }

            List<SQLiteReferenceHandler> handlers = null;
            if (_handlers != null)
            {
                handlers = new List<SQLiteReferenceHandler>();
                foreach (SQLiteReferenceHandler r in _handlers)
                    handlers.Add((SQLiteReferenceHandler)r.Clone());
            }

            SQLiteDeferColumnConstraint deferConstraint = null;
            if (_deferConstraint != null)
                deferConstraint = (SQLiteDeferColumnConstraint)_deferConstraint.Clone();

            SQLiteForeignKeyTableConstraint res = new SQLiteForeignKeyTableConstraint(this.ConstraintName);
            res._columns = columns;
            res._foreignTable = _foreignTable;
            res._foreignColumns = foreignColumns;
            res._handlers = handlers;
            res._deferConstraint = deferConstraint;
            return res;
        }

        private List<SQLiteIndexedColumn> _columns;
        private string _foreignTable;
        private List<SQLiteIndexedColumn> _foreignColumns;
        private List<SQLiteReferenceHandler> _handlers;
        private SQLiteDeferColumnConstraint _deferConstraint;
    }
}
