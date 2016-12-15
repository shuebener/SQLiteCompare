using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteBinaryExpression : SQLiteExpression
    {
        private SQLiteBinaryExpression()
        {
        }

        public SQLiteBinaryExpression(SQLiteExpression left, SQLiteOperator op, SQLiteExpression right)
        {
            if (left.GetType().FullName == "SQLiteParser.SQLiteExpression")
                System.Diagnostics.Debugger.Break();
            _left = left;
            _op = op;
            _right = right;
        }

        public SQLiteExpression LeftExpression
        {
            get { return _left; }
            set 
            {
                if (value.GetType().FullName == "SQLiteParser.SQLiteExpression")
                    System.Diagnostics.Debugger.Break();
                _left = value; 
            }
        }

        public SQLiteOperator BinaryOperator
        {
            get { return _op; }
            set { _op = value; }
        }

        public SQLiteExpression RightExpression
        {
            get { return _right; }
            set { _right = value; }
        }

        public override bool IsConstant(bool allowNull)
        {
            if (_op == SQLiteOperator.Plus || _op == SQLiteOperator.Minus ||
                _op == SQLiteOperator.Star || _op == SQLiteOperator.Slash ||
                _op == SQLiteOperator.Rshift || _op == SQLiteOperator.Lshift ||
                _op == SQLiteOperator.Concat || _op == SQLiteOperator.BitAnd ||
                _op == SQLiteOperator.BitNot || _op == SQLiteOperator.BitOr ||
                _op == SQLiteOperator.Rem)
            {
                if (_left.IsConstant(allowNull) & _right.IsConstant(allowNull))
                    return true;                    
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteBinaryExpression dst = obj as SQLiteBinaryExpression;
            if (dst == null)
                return false;

            if (!RefCompare.CompareMany(_left, dst._left, _right, dst._right))
                return false;

            if (this._op != dst._op)
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            string right;
            if (_right is SQLiteIdExpression || _right is SQLiteTermExpression || _right is SQLiteNameExpression || IsEqualityOperator(_op))
                right = _right.ToString();
            else
                right = "(" + _right.ToString() + ")";

            string left;
            if (_left is SQLiteIdExpression || _left is SQLiteTermExpression || _left is SQLiteNameExpression || IsEqualityOperator(_op))
                left = _left.ToString();
            else
                left = "(" + _left.ToString() + ")";

            string res = left + " " + Utils.GetOperatorString(_op) + " " + right;
            return res;
        }

        public override object Clone()
        {
            SQLiteExpression left = null;
            if (_left != null)
                left = (SQLiteExpression)_left.Clone();
            SQLiteExpression right = null;
            if (_right != null)
                right = (SQLiteExpression)_right.Clone();

            if (left.GetType().FullName == "SQLiteParser.SQLiteExpression")
                System.Diagnostics.Debugger.Break();

            SQLiteBinaryExpression res = new SQLiteBinaryExpression();
            res._left = left;
            res._op = _op;
            res._right = right;
            return res;
        }

        private bool IsEqualityOperator(SQLiteOperator op)
        {
            switch (op)
            {
                case SQLiteOperator.Lt:
                case SQLiteOperator.Gt:
                case SQLiteOperator.Ge:
                case SQLiteOperator.Le:
                case SQLiteOperator.Eq:
                case SQLiteOperator.Ne:
                    return true;
                default:
                    return false;
            } // switch
        }

        private SQLiteExpression _left;
        private SQLiteOperator _op;
        private SQLiteExpression _right;
    }
}

