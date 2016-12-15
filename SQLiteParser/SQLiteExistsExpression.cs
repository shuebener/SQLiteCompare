using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteExistsExpression : SQLiteExpression
    {
        private SQLiteExistsExpression()
        {
        }

        public SQLiteExistsExpression(SQLiteSelectStatement select)
        {
            _select = select;
        }

        public SQLiteSelectStatement SelectStatement
        {
            get { return _select; }
            set { _select = value; }
        }

        public override bool IsConstant(bool allowNull)
        {
            return false;
        } 

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteExistsExpression dst = obj as SQLiteExistsExpression;
            if (dst == null)
                return false;

            if (!RefCompare.Compare(_select, dst._select))
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            string res = "EXISTS (" + _select.ToString() + ")";
            return res;
        }

        public override object Clone()
        {
            SQLiteSelectStatement select = null;
            if (_select != null)
                select = (SQLiteSelectStatement)_select.Clone();

            SQLiteExistsExpression res = new SQLiteExistsExpression();
            res._select = select;
            return res;
        }

        private SQLiteSelectStatement _select;
    }
}
