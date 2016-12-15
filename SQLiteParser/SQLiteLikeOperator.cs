using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteLikeOperator
    {
        private SQLiteLikeOperator()
        {
        }

        public SQLiteLikeOperator(SQLiteLike like, bool negate)
        {
            _like = like;
            _negate = negate;
        }

        public SQLiteLike Like
        {
            get { return _like; }
            set { _like = value; }
        }

        public bool Negate
        {
            get { return _negate; }
            set { _negate = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteLikeOperator dst = obj as SQLiteLikeOperator;
            if (dst == null)
                return false;

            if (_like != dst._like || _negate != dst._negate)
                return false;

            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (_negate)
                sb.Append("NOT ");
            switch (_like)
            {
                case SQLiteLike.Like:
                    sb.Append("LIKE");
                    break;
                case SQLiteLike.Glob:
                    sb.Append("GLOB");
                    break;
                case SQLiteLike.Regexp:
                    sb.Append("REGEXP");
                    break;
                case SQLiteLike.Match:
                    sb.Append("MATCH");
                    break;
                default:
                    throw new ArgumentException("illegal like operator [" + _like.ToString() + "]");
            } // switch

            return sb.ToString();
        }

        public virtual object Clone()
        {
            SQLiteLikeOperator res = new SQLiteLikeOperator();
            res._like = _like;
            res._negate = _negate;
            return res;
        }

        private SQLiteLike _like;
        private bool _negate;
    }

    public enum SQLiteLike
    {
        None = 0,

        Like = 1,

        Glob = 2,

        Regexp = 3,

        Match = 4,
    }
}
