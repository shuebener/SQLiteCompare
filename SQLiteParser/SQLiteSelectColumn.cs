using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteSelectColumn
    {
        public SQLiteSelectColumn(SQLiteExpression expr, string name)
        {
            _expr = expr;
            AsName = name;
        }

        public SQLiteSelectColumn(SQLiteObjectName tableName)
        {
            _tableName = tableName;
        }

        public SQLiteSelectColumn()
        {
        }

        public SQLiteExpression Expression
        {
            get { return _expr; }
            set { _expr = value; }
        }

        public string AsName
        {
            get { return _asName; }
            set { _asName = Utils.QuoteIfNeeded(value); }
        }

        public SQLiteObjectName TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        public bool IsSingleWildcard
        {
            get { return _expr == null && _tableName == null && _asName == null; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteSelectColumn dst = obj as SQLiteSelectColumn;
            if (dst == null)
                return false;

            if (!RefCompare.CompareMany(_expr, dst._expr, _tableName, dst._tableName))
                return false;
            if (_asName != dst._asName)
                return false;

            return true;
        }

        public override string ToString()
        {
            if (_tableName != null)
                return _tableName.ToString() + ".*";
            if (_expr != null && _asName != null)
                return _expr.ToString() + " AS " + _asName;
            else if (_expr != null)
                return _expr.ToString();
            return "*";
        }

        public virtual object Clone()
        {
            SQLiteExpression expr = null;
            if (_expr != null)
                expr = (SQLiteExpression)_expr.Clone();
            SQLiteObjectName tableName = null;
            if (_tableName != null)
                tableName = (SQLiteObjectName)_tableName.Clone();

            SQLiteSelectColumn res = new SQLiteSelectColumn();
            res._expr = expr;
            res._asName = _asName;
            res._tableName = tableName;
            return res;
        }

        private SQLiteExpression _expr;
        private string _asName;
        private SQLiteObjectName _tableName;
    }
}
