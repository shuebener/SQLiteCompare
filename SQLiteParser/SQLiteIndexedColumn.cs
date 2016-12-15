using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteIndexedColumn
    {
        public SQLiteIndexedColumn()
        {
        }

        public SQLiteIndexedColumn(string name, string collation, SQLiteSortOrder order)
        {
            ColumnName = name;
            Collation = collation;
            _order = order;
        }

        public string ColumnName
        {
            get { return _name; }
            set 
            {
                _name = Utils.QuoteIfNeeded(value);
            }
        }

        public string Collation
        {
            get { return _collation; }
            set 
            {
                _collation = Utils.QuoteIfNeeded(value);
            }
        }

        public SQLiteSortOrder SortOrder
        {
            get { return _order; }
            set { _order = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteIndexedColumn dst = obj as SQLiteIndexedColumn;
            if (dst == null)
                return false;

            if (_name != dst._name || _collation != dst._collation || _order != dst._order)
                return false;

            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(_name);
            if (_collation != null)
                sb.Append(" COLLATE " + _collation);
            if (_order == SQLiteSortOrder.Ascending)
                sb.Append(" ASC");
            else if (_order == SQLiteSortOrder.Descending)
                sb.Append(" DESC");

            return sb.ToString();
        }

        public virtual object Clone()
        {
            SQLiteIndexedColumn res = new SQLiteIndexedColumn();
            res._name = _name;
            res._collation = _collation;
            res._order = _order;
            return res;
        }

        private string _name;
        private string _collation;
        private SQLiteSortOrder _order;
    }
}
