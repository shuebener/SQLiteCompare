using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Windows.Forms;

namespace SQLiteTurbo
{
    /// <summary>
    /// This class provides static properties for system-wide configuration
    /// </summary>
    public class Configuration
    {
        #region Public Static Methods
        /// <summary>
        /// Returns the path to the temporary BLOB file (used when reading or writing BLOB
        /// data to databases).
        /// </summary>
        public static string TempBlobFilePath
        {
            get
            {
                if (_tempBlobFilePath != null)
                    return _tempBlobFilePath;
                string appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string sqliteAppdata = appdata + "\\SQLiteCompare\\";
                if (!Directory.Exists(sqliteAppdata))
                    Directory.CreateDirectory(sqliteAppdata);
                _tempBlobFilePath = sqliteAppdata + "blob.dat";
                return _tempBlobFilePath;
            }
        }

        /// <summary>
        /// Returns the path tothe log file
        /// </summary>
        public static string LogFilePath
        {
            get
            {
                string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string sqliteAppdata = appdata + "\\SQLiteCompare\\";
                if (!Directory.Exists(sqliteAppdata))
                    Directory.CreateDirectory(sqliteAppdata);
                return sqliteAppdata + "scompare.log";
            }
        }

        /// <summary>
        /// Used to check if this is the first time the utility is ran
        /// </summary>
        public static bool FirstTime
        {
            get
            {
                object tmp = Registry.GetValue(SQLITE_REG_KEY, FIRST_TIME_VAL, null);
                if (tmp == null)
                    return true;
                else
                    return false;
            }

            set
            {
                Registry.SetValue(SQLITE_REG_KEY, FIRST_TIME_VAL, (value?1:0));
            }
        }

        /// <summary>
        /// Returns the last used left-db path
        /// </summary>
        public static string LastUsedLeftDbPath
        {
            get
            {
                string lpath = (string)Registry.GetValue(SQLITE_REG_KEY, LEFT_DB_REG_VAL, null);
                return lpath;
            }
            set
            {
                Registry.SetValue(SQLITE_REG_KEY, LEFT_DB_REG_VAL, value);
            }
        }

        /// <summary>
        /// Returns the last used right-db path
        /// </summary>
        public static string LastUsedRightDbPath
        {
            get
            {
                string rpath = (string)Registry.GetValue(SQLITE_REG_KEY, RIGHT_DB_REG_VAL, null);
                return rpath;
            }
            set
            {
                Registry.SetValue(SQLITE_REG_KEY, RIGHT_DB_REG_VAL, value);
            }
        }

        /// <summary>
        /// Returns TRUE if the last time a BLOB field comparison was selected.
        /// </summary>
        public static bool LastCompareBlobFields
        {
            get
            {
                object tmp = Registry.GetValue(SQLITE_REG_KEY, BLOB_COMPARE_REG_VAL, 0);
                if (tmp == null || tmp.GetType() != typeof(int))
                    return false;
                int val = (int)tmp;
                if (val == 0)
                    return false;
                else
                    return true;
            }
            set
            {
                Registry.SetValue(SQLITE_REG_KEY, BLOB_COMPARE_REG_VAL, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Returns TRUE if the last time a comparison was performed - it was with data comparison
        /// option turned ON.
        /// </summary>
        public static bool LastComparisonWithData
        {
            get
            {
                object tmp = Registry.GetValue(SQLITE_REG_KEY, DATA_COMPARISON_REG_VAL, 0);
                if (tmp == null || tmp.GetType() != typeof(int))
                    return false;
                int val = (int)tmp;
                if (val == 0)
                    return false;
                else
                    return true;
            }
            set
            {
                Registry.SetValue(SQLITE_REG_KEY, DATA_COMPARISON_REG_VAL, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Get a flag to indicate if we automatically check for software updates on startup.
        /// </summary>
        public static bool CheckUpdatesOnStartup
        {
            get
            {
                object tmp = Registry.GetValue(SQLITE_REG_KEY, CHECK_UPDATES_ON_STARTUP_VAL, 0);
                if (tmp == null || tmp.GetType() != typeof(int))
                    return false;
                int val = (int)tmp;
                if (val == 0)
                    return false;
                return true;
            }
            set
            {
                Registry.SetValue(SQLITE_REG_KEY, CHECK_UPDATES_ON_STARTUP_VAL, value ? 1 : 0);
            }
        }
        #endregion

        #region Constants
        private static string SQLITE_REG_KEY = @"HKEY_CURRENT_USER\SOFTWARE\SQLiteCompare";
        private static string LEFT_DB_REG_VAL = @"LeftDB";
        private static string RIGHT_DB_REG_VAL = @"RightDB";
        private static string FIRST_TIME_VAL = @"FirstTime";
        private static string DATA_COMPARISON_REG_VAL = @"DataComparison";
        private static string BLOB_COMPARE_REG_VAL = @"CompareBlobFields";
        private static string CHECK_UPDATES_ON_STARTUP_VAL = @"CheckUpdatesOnStartup";
        #endregion

        #region Private Variables
        private static string _tempBlobFilePath;
        #endregion
    }
}
