using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteInsertPrefix
    {
        public SQLiteInsertPrefix()
        {
            _isInsert = false;
        }

        public SQLiteInsertPrefix(SQLiteResolveAction conf)
        {
            _isInsert = true;
            _conf = conf;
        }

        public bool IsReplace
        {
            get { return !_isInsert; }
        }

        public bool IsInsert
        {
            get { return _isInsert; }
        }

        public SQLiteResolveAction ResolveAction
        {
            get { return _conf; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteInsertPrefix dst = obj as SQLiteInsertPrefix;
            if (dst == null)
                return false;

            return _conf == dst._conf;
        }

        public virtual object Clone()
        {
            SQLiteInsertPrefix res = new SQLiteInsertPrefix(_conf);
            res._isInsert = _isInsert;
            return res;
        }

        private bool _isInsert = false;
        private SQLiteResolveAction _conf;
    }
}
