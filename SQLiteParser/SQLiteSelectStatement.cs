using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteSelectStatement : SQLiteStatement
    {
        public override object Clone()
        {
            return new SQLiteSelectStatement();
        }
    }
}
