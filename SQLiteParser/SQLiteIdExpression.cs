using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteIdExpression : SQLiteExpression
    {
        public SQLiteIdExpression(string id)
        {
            _id = id;
        }

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public override bool IsConstant(bool allowNull)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteIdExpression dst = obj as SQLiteIdExpression;
            if (dst == null)
                return false;

            if (_id != dst._id)
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            return _id;
        }

        public override object Clone()
        {
            SQLiteExpression res = new SQLiteIdExpression(_id);
            return res;
        }

        private string _id;
    }
}
