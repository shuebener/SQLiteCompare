using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteMultiSelectStatement : SQLiteSelectStatement
    {
        private SQLiteMultiSelectStatement()
        {
        }

        public SQLiteMultiSelectStatement(SQLiteSelectStatement first, SQLiteSelectOperator op, SQLiteSelectStatement next)
        {
            _first = first;
            _op = op;
            _next = next;
        }

        public SQLiteSelectStatement First
        {
            get { return _first; }
            set { _first = value; }
        }

        public SQLiteSelectOperator SelectOperator
        {
            get { return _op; }
            set { _op = value; }
        }

        public SQLiteSelectStatement Next
        {
            get { return _next; }
            set { _next = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteMultiSelectStatement dst = obj as SQLiteMultiSelectStatement;
            if (dst == null)
                return false;

            if (!RefCompare.CompareMany(_first, dst._first, _next, dst._next))
                return false;

            if (_op != dst._op)
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            return _first.ToString() + "\r\n" + Utils.GetSelectOperatorString(_op) + "\r\n" + _next.ToString();
        }

        public override object Clone()
        {
            SQLiteSelectStatement first = null;
            if (_first != null)
                first = (SQLiteSelectStatement)_first.Clone();
            SQLiteSelectStatement next = null;
            if (_next != null)
                next = (SQLiteSelectStatement)_next.Clone();

            SQLiteMultiSelectStatement res = new SQLiteMultiSelectStatement();
            res._first = first;
            res._op = _op;
            res._next = next;
            return res;
        }

        private SQLiteSelectStatement _first;
        private SQLiteSelectOperator _op;
        private SQLiteSelectStatement _next;
    }
}
