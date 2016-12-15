using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteStatement
    {
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteStatement dst = obj as SQLiteStatement;
            if (dst == null)
                return false;

            return true;
        }

        public virtual object Clone()
        {
            return new SQLiteStatement();
        }
    }
}
