using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteBetweenExpression : SQLiteExpression
    {
        public SQLiteBetweenExpression()
        {
        }

        public SQLiteBetweenExpression(SQLiteExpression left, bool between, SQLiteExpression right, SQLiteExpression and)
        {
            _left = left;
            _between = between;
            _right = right;
            _and = and;
        }

        public SQLiteExpression LeftExpression
        {
            get { return _left; }
            set { _left = value; }
        }

        public bool IsBetween
        {
            get { return _between; }
            set { _between = value; }
        }

        public SQLiteExpression RightExpression
        {
            get { return _right; }
            set { _right = value; }
        }

        public SQLiteExpression AndExpression
        {
            get { return _and; }
            set { _and = value; }
        }

        public override bool IsConstant(bool allowNull)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteBetweenExpression dst = obj as SQLiteBetweenExpression;
            if (dst == null)
                return false;

            if (this._between != dst._between)
                return false;

            if (!RefCompare.CompareMany(_left, dst._left, _right, dst._right, _and, dst._and))
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            string res = _left.ToString() + " BETWEEN " + _right.ToString() + " AND " + _and.ToString();
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
            SQLiteExpression and = null;
            if (_and != null)
                and = (SQLiteExpression)_and.Clone();

            SQLiteBetweenExpression res = new SQLiteBetweenExpression();
            res._and = and;
            res._between = _between;
            res._left = left;
            res._right = right;
            return res;
        }

        private SQLiteExpression _left;
        private bool _between;
        private SQLiteExpression _right;
        private SQLiteExpression _and;
    }
}
