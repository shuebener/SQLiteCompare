using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteUpdateStatement : SQLiteStatement
    {
        private SQLiteUpdateStatement()
        {
        }

        public SQLiteUpdateStatement(SQLiteResolveAction conf, string table, 
            List<SQLiteUpdateItem> setlist, SQLiteExpression whereExpr)
        {
            _conf = conf;
            TableName = table;
            _setlist = setlist;
            _whereExpr = whereExpr;
        }

        public SQLiteResolveAction ConflictAction
        {
            get { return _conf; }
            set { _conf = value; }
        }

        public string TableName
        {
            get { return _table; }
            set 
            {
                _table = Utils.QuoteIfNeeded(value);
            }
        }

        public List<SQLiteUpdateItem> SetList
        {
            get { return _setlist; }
            set { _setlist = value; }
        }

        public SQLiteExpression WhereExpression
        {
            get { return _whereExpr; }
            set { _whereExpr = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteUpdateStatement dst = obj as SQLiteUpdateStatement;
            if (dst == null)
                return false;

            if (_conf != dst._conf || _table != dst._table)
                return false;
            if (!RefCompare.Compare(_whereExpr, dst._whereExpr))
                return false;
            if (!RefCompare.CompareList<SQLiteUpdateItem>(_setlist, dst._setlist))
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            // UPDATE orconf nm SET setlist where_opt
            sb.Append("UPDATE");
            if (_conf != SQLiteResolveAction.None)
                sb.Append(" OR " + Utils.GetResolveActionString(_conf));
            sb.Append(" " + _table + "\r\n    SET ");
            for (int i = 0; i < _setlist.Count; i++)
            {
                sb.Append(_setlist[i].ToString());
                if (i < _setlist.Count - 1)
                    sb.Append(",\r\n        ");
            } // for
            if (_whereExpr != null)
                sb.Append("\r\n    WHERE " + _whereExpr.ToString());

            return sb.ToString();
        }

        public override object Clone()
        {
            List<SQLiteUpdateItem> setlist = null;
            if (_setlist != null)
            {
                setlist = new List<SQLiteUpdateItem>();
                foreach (SQLiteUpdateItem item in _setlist)
                    setlist.Add((SQLiteUpdateItem)item.Clone());
            }
            SQLiteExpression whereExpr = null;
            if (_whereExpr != null)
                whereExpr = (SQLiteExpression)_whereExpr.Clone();

            SQLiteUpdateStatement res = new SQLiteUpdateStatement();
            res._conf = _conf;
            res._setlist = setlist;
            res._table = _table;
            res._whereExpr = whereExpr;
            return res;
        }

        private SQLiteResolveAction _conf;
        private string _table;
        private List<SQLiteUpdateItem> _setlist;
        private SQLiteExpression _whereExpr;
    }
}
