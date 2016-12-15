using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteLikeExpression : SQLiteExpression
    {
        private SQLiteLikeExpression()
        {
        }

        public SQLiteLikeExpression(SQLiteExpression left, SQLiteLikeOperator likeOp, SQLiteExpression right, SQLiteExpression escape)
        {
            _left = left;
            _likeOp = likeOp;
            _right = right;
            _escape = escape;
        }

        public SQLiteExpression LeftExpression
        {
            get { return _left; }
            set { _left = value; }
        }

        public SQLiteLikeOperator LikeOperator
        {
            get { return _likeOp; }
            set { _likeOp = value; }
        }

        public SQLiteExpression RightExpression
        {
            get { return _right; }
            set { _right = value; }
        }

        public SQLiteExpression EscapeExpression
        {
            get { return _escape; }
            set { _escape = value; }
        }

        public override bool IsConstant(bool allowNull)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteLikeExpression dst = obj as SQLiteLikeExpression;
            if (dst == null)
                return false;

            if (!RefCompare.CompareMany(_likeOp, dst._likeOp, _left, dst._left, _right, dst._right, _escape, dst._escape))
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            // expr likeop expr escape

            StringBuilder sb = new StringBuilder();
            sb.Append(_left.ToString() + " " + _likeOp.ToString() + " " + _right.ToString());
            if (_escape != null)
                sb.Append(" ESCAPE " + _escape.ToString());

            return sb.ToString();
        }

        public override object Clone()
        {
            SQLiteLikeOperator likeOp = null;
            if (_likeOp != null)
                likeOp = (SQLiteLikeOperator)_likeOp.Clone();
            SQLiteExpression left = null;
            if (_left != null)
                left = (SQLiteExpression)_left.Clone();
            SQLiteExpression right = null;
            if (_right != null)
                right = (SQLiteExpression)_right.Clone();
            SQLiteExpression escape = null;
            if (_escape != null)
                escape = (SQLiteExpression)_escape.Clone();

            SQLiteLikeExpression res = new SQLiteLikeExpression();
            res._likeOp = likeOp;
            res._left = left;
            res._right = right;
            res._escape = escape;
            return res;
        }

        private SQLiteLikeOperator _likeOp;
        private SQLiteExpression _left;
        private SQLiteExpression _right;
        private SQLiteExpression _escape;
    }
}
