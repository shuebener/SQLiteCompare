using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteSchema
    {
        public void AddStatement(SQLiteDdlStatement stmt)
        {
            if (stmt is SQLiteCreateTableStatement)
                Tables.Add(stmt.ObjectName, (SQLiteCreateTableStatement)stmt);
            else if (stmt is SQLiteCreateIndexStatement)
                Indexes.Add(stmt.ObjectName, (SQLiteCreateIndexStatement)stmt);
            else if (stmt is SQLiteCreateViewStatement)
                Views.Add(stmt.ObjectName, (SQLiteCreateViewStatement)stmt);
            else if (stmt is SQLiteCreateTriggerStatement)
                Triggers.Add(stmt.ObjectName, (SQLiteCreateTriggerStatement)stmt);
            else
                throw new ArgumentException("Illegal statement type");
        }

        public Dictionary<SQLiteObjectName, SQLiteCreateTableStatement> Tables = new Dictionary<SQLiteObjectName, SQLiteCreateTableStatement>();
        public Dictionary<SQLiteObjectName, SQLiteCreateIndexStatement> Indexes = new Dictionary<SQLiteObjectName, SQLiteCreateIndexStatement>();
        public Dictionary<SQLiteObjectName, SQLiteCreateViewStatement> Views = new Dictionary<SQLiteObjectName, SQLiteCreateViewStatement>();
        public Dictionary<SQLiteObjectName, SQLiteCreateTriggerStatement> Triggers = new Dictionary<SQLiteObjectName, SQLiteCreateTriggerStatement>();
    }
}
