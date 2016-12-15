using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;

namespace CreateTestDatabases
{
    public class CreateDataChecks_1 : IDatabasePairCreator
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
                    @"    tblob TINYBLOB, " +
                    @"    blob BLOB, " +
                    @"    mblob MEDIUMBLOB, " +
                    @"    ba BYTEA" +
                    @" )", c1);

                byte[] f1 = new byte[] { 1 };
                byte[] f2 = new byte[] { 1, 2 };
                byte[] f3 = new byte[] { 1, 2, 3 };
                byte[] f4 = new byte[] { 1, 2, 3, 4 };

                byte[][] tarr = new byte[][] { f1, f2, f3, f4 };

                SQLiteTransaction tx1 = c1.BeginTransaction();
                InsertData(tarr, c1, tx1);
                tx1.Commit();

                File.Copy(db1, db2);
            } // using
        }

        private void InsertData(byte[][] tarr, SQLiteConnection conn, SQLiteTransaction tx)
        {
            Random rand = new Random();
            SQLiteCommand insert = new SQLiteCommand("INSERT INTO test1 (tblob,blob,mblob,ba) VALUES (@f1, @f2, @f3, @f4)", conn, tx);
            insert.Parameters.Add("@f1", System.Data.DbType.Binary);
            insert.Parameters.Add("@f2", System.Data.DbType.Binary);
            insert.Parameters.Add("@f3", System.Data.DbType.Binary);
            insert.Parameters.Add("@f4", System.Data.DbType.Binary);

            for (int i = 0; i < 100; i++)
            {
                for (int k = 1; k <= tarr.Length; k++)
                {
                    int index = rand.Next(0, tarr.Length - 1);
                    insert.Parameters["@f" + k].Value = tarr[index];
                }
                insert.ExecuteNonQuery();
            } // for
        }

        #endregion
    }
}
