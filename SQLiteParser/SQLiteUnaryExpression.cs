using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteUnaryExpression : SQLiteExpression
    {
        private SQLiteUnaryExpression()
        {
        }

        public SQLiteUnaryExpression(SQLiteOperator op, SQLiteExpression expr)
        {
            _op = op;
            _expr = expr;
        }

        public SQLiteOperator UnaryOperator
        {
            get { return _op; }
            set { _op = value; }
        }

        public SQLiteExpression Expression
        {
            get { return _expr; }
            set { _expr = value; }
        }

        public override bool IsConstant(bool allowNull)
        {
            return _expr.IsConstant(allowNull);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteUnaryExpression dst = obj as SQLiteUnaryExpression;
            if (dst == null)
                return false;

            if (!RefCompare.Compare(_expr, dst._expr))
                return false;
            if (_op != dst._op)
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            string res = null;
            if (_op == SQLiteOperator.Is_Not_Null || _op == SQLiteOperator.Is_Null ||
                _op == SQLiteOperator.IsNull || _op == SQLiteOperator.Not_Null ||
                _op == SQLiteOperator.NotNull || _op == SQLiteOperator.Not)
                res = _expr.ToString() + " " + Utils.GetOperatorString(_op) + " ";
            else
                res = Utils.GetOperatorString(_op) + _expr.ToString();
            return res;
        }

        public override object Clone()
        {
            SQLiteExpression expr = null;
            if (_expr != null)
                expr = (SQLiteExpression)_expr.Clone();

            SQLiteUnaryExpression res = new SQLiteUnaryExpression();
            res._expr = expr;
            res._op = _op;
            return res;
        }

        private SQLiteOperator _op;
        private SQLiteExpression _expr;
    }
}
