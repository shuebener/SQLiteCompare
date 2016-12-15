using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteTermExpression : SQLiteExpression
    {
        private SQLiteTermExpression()
        {
        }

        public SQLiteTermExpression(SQLiteTerm term)
        {
            _term = term;
        }

        public SQLiteTerm Term
        {
            get { return _term; }
            set { _term = value; }
        }

        public override bool IsConstant(bool allowNull)
        {
            return !_term.AsTimeFunction.HasValue;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteTermExpression dst = obj as SQLiteTermExpression;
            if (dst == null)
                return false;

            if (!RefCompare.Compare(_term, dst._term))
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            return _term.ToString();
        }

        public override object Clone()
        {
            SQLiteTerm term = null;
            if (_term != null)
                term = (SQLiteTerm)_term.Clone();

            SQLiteTermExpression res = new SQLiteTermExpression();
            res._term = term;
            return res;
        }

        private SQLiteTerm _term;
    }
}
