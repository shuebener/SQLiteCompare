using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;

namespace CreateTestDatabases
{
    public class Utils
    {
        public static SQLiteConnection CreateConnection(string dbpath)
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = dbpath;
            return new SQLiteConnection(builder.ConnectionString);
        }

        public static void RunCommand(string command, params SQLiteConnection[] clist)
        {
            foreach (SQLiteConnection conn in clist)
            {
                SQLiteCommand cmd = new SQLiteCommand(command, conn);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
