using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteCreateIndexStatement : SQLiteDdlStatement
    {
        private SQLiteCreateIndexStatement(SQLiteObjectName indexName)
            : base(indexName)
        {
        }

        public SQLiteCreateIndexStatement(bool isUnique, SQLiteObjectName indexName, bool ifNotExists, 
            string onTable, List<SQLiteIndexedColumn> columns)
            : base(indexName)
        {
            _isUnique = isUnique;
            _ifNotExists = ifNotExists;
            OnTable = onTable;
            _columns = columns;
        }

        public bool IsUnique
        {            
            get { return _isUnique; }
            set { _isUnique = value; }
        }

        public bool IfNotExists
        {
            get { return _ifNotExists; }
            set { _ifNotExists = value; }
        }

        public string OnTable
        {
            get { return _onTable; }
            set 
            {
                _onTable = Utils.QuoteIfNeeded(value);
            }
        }

        public List<SQLiteIndexedColumn> IndexedColumns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteCreateIndexStatement dst = obj as SQLiteCreateIndexStatement;
            if (dst == null)
                return false;

            if (_isUnique != dst._isUnique ||
                _ifNotExists != dst._ifNotExists ||
                _onTable != dst._onTable)
                return false;

            if (!RefCompare.CompareList<SQLiteIndexedColumn>(_columns, dst._columns))
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            return ToStatement(this.ObjectName.ToString(), _onTable);
        }

        public string ToStatement(string indexName, string tableName)
        {
            // CREATE uniqueflag INDEX ifnotexists nm dbnm ON nm LP idxlist RP
            StringBuilder sb = new StringBuilder();
            sb.Append("CREATE");
            if (_isUnique)
                sb.Append(" UNIQUE");
            sb.Append(" INDEX");
            if (_ifNotExists)
                sb.Append(" IF NOT EXISTS");
            sb.Append(" " + Utils.QuoteIfNeeded(indexName) + "\r\nON " + Utils.QuoteIfNeeded(tableName));
            sb.Append("\r\n(");
            for (int i = 0; i < _columns.Count; i++)
            {
                sb.Append("\r\n    " + _columns[i].ToString());
                if (i < _columns.Count - 1)
                    sb.Append(",");
            } // for
            sb.Append("\r\n)");

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

            SQLiteObjectName indexName = null;
            if (this.ObjectName != null)
                indexName = (SQLiteObjectName)this.ObjectName.Clone();

            SQLiteCreateIndexStatement res = new SQLiteCreateIndexStatement(indexName);
            res._columns = columns;
            res._ifNotExists = _ifNotExists;
            res._isUnique = _isUnique;
            res._onTable = _onTable;
            return res;
        }

        private bool _isUnique;
        private bool _ifNotExists;
        private string _onTable;
        private List<SQLiteIndexedColumn> _columns;
    }
}
