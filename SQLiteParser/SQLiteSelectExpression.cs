using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteSelectExpression : SQLiteExpression
    {
        private SQLiteSelectExpression()
        {
        }

        public SQLiteSelectExpression(SQLiteSelectStatement select)
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

            SQLiteSelectExpression dst = obj as SQLiteSelectExpression;
            if (dst == null)
                return false;

            if (!RefCompare.Compare(_select, dst._select))
                return false;

            return base.Equals(obj);
        }

        public override object Clone()
        {
            SQLiteSelectStatement select = null;
            if (_select != null)
                select = (SQLiteSelectStatement)_select.Clone();

            SQLiteSelectExpression res = new SQLiteSelectExpression();
            res._select = select;
            return res;
        }

        public override string ToString()
        {
            return "("+_select.ToString()+")";
        }

        private SQLiteSelectStatement _select;
    }
}
