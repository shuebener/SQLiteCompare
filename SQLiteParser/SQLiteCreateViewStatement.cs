using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteCreateViewStatement : SQLiteDdlStatement
    {
        private SQLiteCreateViewStatement(SQLiteObjectName viewName)
            : base(viewName)
        {
        }

        public SQLiteCreateViewStatement(bool isTemp, bool ifNotExists, SQLiteObjectName viewName, SQLiteSelectStatement select)
            : base(viewName)
        {
            _isTemp = isTemp;
            _ifNotExists = ifNotExists;
            _select = select;
        }

        public bool IsTemp
        {
            get { return _isTemp; }
            set { _isTemp = value; }
        }

        public bool IfNotExists
        {
            get { return _ifNotExists; }
            set { _ifNotExists = value; }
        }

        public SQLiteSelectStatement SelectStatement
        {
            get { return _select; }
            set { _select = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteCreateViewStatement dst = obj as SQLiteCreateViewStatement;
            if (dst == null)
                return false;

            if (_isTemp != dst._isTemp || _ifNotExists != dst._ifNotExists)
                return false;

            if (!RefCompare.Compare(_select, dst._select))
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            // CREATE temp VIEW ifnotexists nm dbnm AS select

            StringBuilder sb = new StringBuilder();
            sb.Append("CREATE");
            if (_isTemp)
                sb.Append(" TEMP");
            sb.Append(" VIEW");
            if (_ifNotExists)
                sb.Append(" IF NOT EXISTS");
            sb.Append(" " + base.ObjectName.ToString() + "\r\nAS\r\n" + _select.ToString());

            return sb.ToString();
        }

        public override object Clone()
        {
            SQLiteSelectStatement select = null;
            if (_select != null)
                select = (SQLiteSelectStatement)_select.Clone();
            SQLiteObjectName viewName = null;
            if (this.ObjectName != null)
                viewName = (SQLiteObjectName)this.ObjectName.Clone();

            SQLiteCreateViewStatement res = new SQLiteCreateViewStatement(viewName);
            res._ifNotExists = _ifNotExists;
            res._isTemp = _isTemp;
            res._select = select;
            return res;
        }

        private bool _isTemp;
        private bool _ifNotExists;
        private SQLiteSelectStatement _select;
    }
}
