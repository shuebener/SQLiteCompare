using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.IO;

namespace CreateTestDatabases
{
    public class CreateDataChecks_3 : IDatabasePairCreator
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
                    "CREATE TABLE log (" +
                    " id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                    " user_id INTEGER NULL DEFAULT NULL," +
                    " type_id INTEGER NOT NULL," +
                    " severity_id TINYINT NOT NULL," +
                    " message VARCHAR(255) NOT NULL," +
                    " data LONGBLOB NULL," +
                    " timestamp DATETIME NOT NULL," +
                    " ip INTEGER UNSIGNED NOT NULL" +
                    " )", c1);

                Utils.RunCommand(
                    "CREATE TABLE test1 (" +
                    "     a," +
                    "     b COLLATE BINARY," +
                    "     c COLLATE RTRIM," +
                    "     d COLLATE NOCASE" +
                    ")", c1);

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
            SQLiteCommand insert1 = new SQLiteCommand("INSERT INTO log " +
                "(user_id,type_id,severity_id,message,data, timestamp,ip) VALUES " +
                "(1,2,3,'message',X'01020304','2004-12-12',4)", conn, tx);

            SQLiteCommand insert2 = new SQLiteCommand("INSERT INTO test1 (a,b,c,d) VALUES " +
                    "(1,2,'c','d')", conn, tx);

            for (int i = 0; i < 100; i++)
            {
                insert1.ExecuteNonQuery();
                insert2.ExecuteNonQuery();
            } // for
        }
    }
}
