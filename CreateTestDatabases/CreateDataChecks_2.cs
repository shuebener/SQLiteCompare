using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.IO;

namespace CreateTestDatabases
{
    public class CreateDataChecks_2 : IDatabasePairCreator
    {
        #region IDatabasePairCreator Members

        public void CreatePair(string db1, string db2)
        {
            if (File.Exists(db1))
                File.Delete(db1);
            if (File.Exists(db2))
                File.Delete(db2);
            SQLiteConnection.CreateFile(db1);

            using (SQLiteConnection c1 = Utils.CreateConnection(db1))
            {
                c1.Open();

                Utils.RunCommand(
                    @" CREATE TABLE test1 " +
                    @" (" +
                    @"    ascending integer NOT NULL PRIMARY KEY," +
                    @"    t1 char," +
                    @"    t2 char(10),"+
                    @"    t3 varchar,"+
                    @"    t4 varchar(50),"+
                    @"    t5 nchar,"+
                    @"    t6 nchar(10),"+
                    @"    t7 text,"+
                    @"    t8 tinytext,"+
                    @"    t9 mediumtext,"+
                    @"    t10 longtext"+
                    @" )", c1);

                SQLiteTransaction tx1 = c1.BeginTransaction();
                InsertData(c1, tx1);
                tx1.Commit();

                File.Copy(db1, db2);
            } // using
        }

        #endregion

        private void InsertData(SQLiteConnection conn, SQLiteTransaction tx)
        {
            Random rand = new Random();
            SQLiteCommand insert = new SQLiteCommand("INSERT INTO test1 " +
                "(t1,t2,t3,t4,t5,t6,t7,t8,t9,t10) VALUES " +
                "('a','b','c','d','e','f','e','g','h','i')", conn, tx);

            for (int i = 0; i < 100; i++)
            {
                insert.ExecuteNonQuery();
            } // for
        }
    }
}
