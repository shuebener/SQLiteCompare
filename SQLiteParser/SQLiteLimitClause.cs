using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteLimitClause
    {
        private SQLiteLimitClause()
        {
        }

        public SQLiteLimitClause(SQLiteExpression limit)
        {
            _limit = limit;
        }

        public SQLiteLimitClause(SQLiteExpression limit, SQLiteExpression offset)
        {
            _limit = limit;
            _offset = offset;
        }

        public SQLiteExpression LimitExpression
        {
            get { return _limit; }
            set { _limit = value; }
        }

        public SQLiteExpression OffsetExpression
        {
            get { return _offset; }
            set { _offset = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteLimitClause dst = obj as SQLiteLimitClause;
            if (dst == null)
                return false;

            if (!RefCompare.Compare(_limit, dst._limit))
                return false;
            if (!RefCompare.Compare(_offset, dst._offset))
                return false;

            return true;
        }

        public override string ToString()
        {
            if (_offset == null)
                return "LIMIT " + _limit.ToString();
            return "LIMIT " + _limit.ToString() + " OFFSET " + _offset.ToString();
        }

        public virtual object Clone()
        {
            SQLiteExpression limit = null;
            if (_limit != null)
                limit = (SQLiteExpression)_limit.Clone();
            SQLiteExpression offset = null;
            if (_offset != null)
                offset = (SQLiteExpression)_offset.Clone();

            SQLiteLimitClause res = new SQLiteLimitClause();
            res._limit = limit;
            res._offset = offset;
            return res;
        }

        private SQLiteExpression _limit;
        private SQLiteExpression _offset;
    }

}
