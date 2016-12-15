using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLitePrimaryKeyColumnConstraint : SQLiteColumnConstraint
    {
        private SQLitePrimaryKeyColumnConstraint(string name)
            : base(name)
        {
        }

        public SQLitePrimaryKeyColumnConstraint(string name, SQLiteSortOrder order, SQLiteResolveAction conf, bool autoincrement)
            : base(name)
        {
            _order = order;
            _conf = conf;
            _autoincrement = autoincrement;
        }

        public bool IsAutoincrement
        {
            get { return _autoincrement; }
            set { _autoincrement = value; }
        }

        public SQLiteSortOrder SortOrder
        {
            get { return _order; }
            set { _order = value; }
        }

        public SQLiteResolveAction ResolveAction
        {
            get { return _conf; }
            set { _conf = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLitePrimaryKeyColumnConstraint dst = obj as SQLitePrimaryKeyColumnConstraint;
            if (dst == null)
                return false;

            if (_autoincrement != dst._autoincrement || _order != dst._order || _conf != dst._conf)
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (ConstraintName != null)
                sb.Append("CONSTRAINT " + ConstraintName + " ");
            sb.Append("PRIMARY KEY");
            if (_order == SQLiteSortOrder.Ascending)
                sb.Append(" ASC");
            else if (_order == SQLiteSortOrder.Descending)
                sb.Append(" DESC");
            if (_conf != SQLiteResolveAction.None)
                sb.Append(" " + Utils.GetConflictClauseString(_conf));
            if (_autoincrement)
                sb.Append(" AUTOINCREMENT");
            return sb.ToString();
        }

        public override object Clone()
        {
            SQLitePrimaryKeyColumnConstraint res = new SQLitePrimaryKeyColumnConstraint(ConstraintName);
            res._autoincrement = _autoincrement;
            res._order = _order;
            res._conf = _conf;
            return res;
        }

        private bool _autoincrement;
        private SQLiteSortOrder _order;
        private SQLiteResolveAction _conf;
    }

    public enum SQLiteSortOrder
    {
        None = 0,

        Ascending = 1,

        Descending = 2,
    }
}
