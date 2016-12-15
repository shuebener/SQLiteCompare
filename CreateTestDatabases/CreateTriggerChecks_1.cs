using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.IO;


namespace CreateTestDatabases
{
    public class CreateTriggerChecks_1 : IDatabasePairCreator
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
                    "CREATE TABLE exam (ekey      INTEGER PRIMARY KEY," +
                        "fn        VARCHAR(15),                             " +
                        "ln        VARCHAR(30),                             " +
                        "exam      INTEGER,                                 " +
                        "score     DOUBLE,                                  " +
                        "timeEnter DATE)", c1);

                Utils.RunCommand(
                    "CREATE TRIGGER insert_exam_timeEnter AFTER  INSERT ON exam            " +
                        "BEGIN                                                                 " +
                        " UPDATE exam SET timeEnter = DATETIME('NOW')                           " +
                        " WHERE rowid = new.rowid;                                     " +
                        "END", c1);

                Utils.RunCommand(
                    "CREATE TABLE examlog (" +
                    "  lkey INTEGER PRIMARY KEY, " +
                    "  ekey INTEGER, " +
                    "  ekeyOLD INTEGER, " +
                    "  fnNEW   VARCHAR(15)," +
                    "  fnOLD   VARCHAR(15), " +
                    "  lnNEW   VARCHAR(30), " +
                    "  lnOLD   VARCHAR(30), " +
                    "  examNEW INTEGER, " +
                    "  examOLD INTEGER, " +
                    "  scoreNEW DOUBLE, " +
                    "  scoreOLD DOUBLE, " +
                    "  sqlAction VARCHAR(15), " +
                    "  examtimeEnter    DATE, " +
                    "  [sort $it] CONSTRAINT sortit CHECK([sort $it] > 5),"+
                    "  examtimeUpdate   DATE, " +
                    "  timeEnter        DATE)", c1);

                Utils.RunCommand(
                    "CREATE TRIGGER update_examlog AFTER UPDATE  ON exam                   " +
                    " BEGIN                                                                 " +
                    "   INSERT INTO examlog  (ekey,ekeyOLD,fnOLD,fnNEW,lnOLD,               " +
                             "lnNEW,examOLD,examNEW,scoreOLD,               " +
                             "scoreNEW,sqlAction,examtimeEnter,             " +
                             "[sort $it],"+
                             "examtimeUpdate,timeEnter)                     " +
                        "values (new.ekey,old.ekey,old.fn,new.fn,old.ln,             " +
                        " new.ln,old.exam, new.exam,old.score,                " +
                       " new.score, 'UPDATE',old.timeEnter,                  " +
                       "NULL,"+
                        "DATETIME('NOW'),DATETIME('NOW') ); " +
                    "END", c1);

                Utils.RunCommand(
                    "CREATE TRIGGER insert_examlog AFTER INSERT ON exam                    " +
                    " BEGIN                                                                 " +
                    " INSERT INTO examlog  (ekey,fnNEW,lnNEW,examNEW,scoreNEW,              " +
                           "sqlAction,examtimeEnter,timeEnter)              " +
                    " values (new.ekey,new.fn,new.ln,new.exam,new.score,          " +
                       "'INSERT',new.timeEnter,DATETIME('NOW') );           " +
                    " END                 ", c1);

                Utils.RunCommand(
                    "CREATE TRIGGER delete_examlog DELETE ON exam                          " +
                    " BEGIN                                                                 " +
                    "  INSERT INTO examlog  (ekey,fnOLD,lnNEW,examOLD,scoreOLD,              " +
                           "sqlAction,timeEnter)                            " +
                    "     values (old.ekey,old.fn,old.ln,old.exam,old.score,          " +
                    "   'DELETE',DATETIME('NOW') ); " +
                    " END", c1);

                Utils.RunCommand(
                    "CREATE TABLE foo ("+
                    " id INTEGER NOT NULL PRIMARY KEY"+
                    ")", c1);

                Utils.RunCommand(
                    "CREATE TABLE bar (" +
                    " id INTEGER NOT NULL PRIMARY KEY," +
                    " fooId INTEGER CONSTRAINT fk_foo_id REFERENCES foo(id)," +
                    " fooId2 INTEGER NOT NULL CONSTRAINT fk_foo_id2 REFERENCES foo(id) ON DELETE CASCADE" +
                    ")", c1);

                File.Copy(db1, db2);
            } // using
        }

        #endregion
    }
}
