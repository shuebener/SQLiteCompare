using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteCastExpression : SQLiteExpression
    {
        private SQLiteCastExpression()
        {
        }

        public SQLiteCastExpression(SQLiteExpression expr, SQLiteColumnType type)
        {
            _expr = expr;
            _type = type;
        }

        public SQLiteExpression Expression
        {
            get { return _expr; }
            set { _expr = value; }
        }

        public SQLiteColumnType CastType
        {
            get { return _type; }
            set { _type = value; }
        }

        public override bool IsConstant(bool allowNull)
        {
            return _expr.IsConstant(allowNull);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteCastExpression dst = obj as SQLiteCastExpression;
            if (dst == null)
                return false;

            if (!RefCompare.CompareMany(_expr, dst._expr, _type, dst._type))
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            string res = "CAST (" + _expr.ToString() + " AS " + _type.ToString() + ")";
            return res;
        }

        public override object Clone()
        {
            SQLiteExpression expr = null;
            if (_expr != null)
                expr = (SQLiteExpression)_expr.Clone();
            SQLiteColumnType type = null;
            if (_type != null)
                type = (SQLiteColumnType)_type.Clone();

            SQLiteCastExpression res = new SQLiteCastExpression();
            res._expr = expr;
            res._type = type;
            return res;
        }

        private SQLiteExpression _expr;
        private SQLiteColumnType _type;
    }
}
