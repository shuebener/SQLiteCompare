using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteNullExpression : SQLiteExpression
    {
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteNullExpression dst = obj as SQLiteNullExpression;
            if (dst == null)
                return false;

            return true;
        }

        public override bool IsConstant(bool allowNull)
        {
            return allowNull;
        }

        public override string ToString()
        {
            return "NULL";
        }

        public override object Clone()
        {
            SQLiteExpression res = new SQLiteNullExpression();
            return res;
        }
    }
}
