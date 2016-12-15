using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteDdlMain
    {
        public static SQLiteDdlStatement GetStatement()
        {
            if (CreateTable != null)
                return _createTable;
            if (CreateIndex != null)
                return _createIndex;
            if (CreateView != null)
                return _createView;
            if (CreateTrigger != null)
                return _createTrigger;
            throw new ApplicationException("no sql statement was found");
        }

        public static SQLiteCreateTableStatement CreateTable
        {
            get { return _createTable; }
            set
            {
                _createTable = value;
                _createView = null;
                _createTrigger = null;
                _createIndex = null;
            }
        }

        public static SQLiteCreateIndexStatement CreateIndex
        {
            get { return _createIndex; }
            set
            {
                _createIndex = value;
                _createView = null;
                _createTrigger = null; 
                _createTable = null;
            }
        }

        public static SQLiteCreateViewStatement CreateView
        {
            get { return _createView; }
            set
            {
                _createView = value;
                _createTable = null;
                _createTrigger = null; 
                _createIndex = null;
            }
        }

        public static SQLiteCreateTriggerStatement CreateTrigger
        {
            get { return _createTrigger; }
            set
            {
                _createTrigger = value;
                _createTable = null;
                _createIndex = null; 
                _createView = null;
            }
        }

        private static SQLiteCreateIndexStatement _createIndex;
        private static SQLiteCreateTableStatement _createTable;
        private static SQLiteCreateTriggerStatement _createTrigger;
        private static SQLiteCreateViewStatement _createView;
    }
}
