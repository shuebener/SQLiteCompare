using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteNameExpression : SQLiteExpression
    {
        private SQLiteNameExpression()
        {
        }

        public SQLiteNameExpression(SQLiteObjectName name)
        {
            _name = name;
        }

        public SQLiteObjectName Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public override bool IsConstant(bool allowNull)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteNameExpression dst = obj as SQLiteNameExpression;
            if (dst == null)
                return false;

            if (!RefCompare.Compare(_name, dst._name))
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            return _name.ToString();
        }

        public override object Clone()
        {            
            SQLiteObjectName name = null;
            if (_name != null)
                name = (SQLiteObjectName)_name.Clone();

            SQLiteNameExpression res = new SQLiteNameExpression();
            res._name = name;
            return res;
        }

        private SQLiteObjectName _name;
    }
}
