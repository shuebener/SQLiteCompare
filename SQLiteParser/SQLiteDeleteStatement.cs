using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteDeleteStatement : SQLiteStatement
    {
        private SQLiteDeleteStatement()
        {
        }

        public SQLiteDeleteStatement(string table, SQLiteExpression whereExpr)
        {
            TableName = table;
            _whereExpr = whereExpr;
        }

        public string TableName
        {
            get { return _table; }
            set 
            {
                _table = Utils.QuoteIfNeeded(value);
            }
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

            SQLiteDeleteStatement dst = obj as SQLiteDeleteStatement;
            if (dst == null)
                return false;

            if (_table != dst._table)
                return false;
            if (!RefCompare.Compare(_whereExpr, dst._whereExpr))
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM " + _table);
            if (_whereExpr != null)
                sb.Append("\r\n    WHERE " + _whereExpr.ToString());
            return sb.ToString();
        }

        public override object Clone()
        {
            SQLiteExpression whereExpr = null;
            if (_whereExpr != null)
                whereExpr = (SQLiteExpression)_whereExpr.Clone();

            SQLiteDeleteStatement res = new SQLiteDeleteStatement();
            res._table = _table;
            res._whereExpr = whereExpr;
            return res;
        }

        private string _table;
        private SQLiteExpression _whereExpr;
    }
}
