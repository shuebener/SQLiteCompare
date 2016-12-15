using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteTurbo
{
    /// <summary>
    /// Contains the information for a single difference between a row
    /// that exists in the left database and a row that exists in the right
    /// database.
    /// </summary>
    public class TableChangeItem
    {
        /// <summary>
        /// Contains the result of the row comparison
        /// </summary>
        public ComparisonResult Result;

        /// <summary>
        /// Contains an array of column names (it gives the name of a column
        /// according to its index).
        /// </summary>
        public string[] LeftColumnNames;

        /// <summary>
        /// Contains an array of column names for the right database table
        /// </summary>
        public string[] RightColumnNames;

        /// <summary>
        /// Contains the values of the left table fields.
        /// </summary>
        public object[] LeftFields;

        /// <summary>
        /// Contains the values of the right table fields.
        /// </summary>
        public object[] RightFields;

        /// <summary>
        /// Contains a map from a column name to its index
        /// </summary>
        public Dictionary<string, int> LeftFieldIndexes;

        /// <summary>
        /// Contains a map from a column name to its index
        /// </summary>
        public Dictionary<string, int> RightFieldIndexes;

        /// <summary>
        /// Tag used by client application
        /// </summary>
        public object Tag;

        /// <summary>
        /// Used to locate the change item in its difference table if necessary
        /// </summary>
        public long ChangeItemRowId = -1;

        /// <summary>
        /// Used to locate the row in the left database table
        /// </summary>
        public long LeftRowId = -1;

        /// <summary>
        /// Used to locate the row in the right database table
        /// </summary>
        public long RightRowId = -1;

        /// <summary>
        /// In case the comparison included BLOB fields - this parameter provides
        /// a list of all BLOB column names where changed were detected betweeb
        /// the two databases.
        /// </summary>
        public List<string> ChangedBlobsColumnNames = null;

        public object GetField(string name, bool isLeft)
        {
            object[] fields;
            Dictionary<string, int> indexes;

            if (isLeft)
            {
                fields = LeftFields;
                indexes = LeftFieldIndexes;
            }
            else
            {
                fields = RightFields;
                indexes = RightFieldIndexes;
            }

            return fields[indexes[name.ToLower()]];
        }

        public bool HasField(string name, bool isLeft)
        {
            if (isLeft && LeftFieldIndexes.ContainsKey(name.ToLower()))
                return true;
            else if (!isLeft && RightFieldIndexes.ContainsKey(name.ToLower()))
                return true;
            else
                return false;
        }

        public void SetField(string columnName, bool isLeft, object updatedValue)
        {
            object[] fields;
            string[] names;

            if (isLeft)
            {
                fields = LeftFields;
                names = LeftColumnNames;
            }
            else
            {
                fields = RightFields;
                names = RightColumnNames;
            }

            for (int i = 0; i < names.Length; i++)
            {
                if (SQLiteParser.Utils.Chop(names[i]).ToLower() == 
                    SQLiteParser.Utils.Chop(columnName).ToLower())
                {
                    fields[i] = updatedValue;
                    break;
                }
            }

            if (LeftColumnNames != null && RightColumnNames != null)
            {
                Result = ComparisonResult.Same;
                for (int i = 0; i < LeftColumnNames.Length; i++)
                {
                    string cname = LeftColumnNames[i];
                    for (int k = 0; k < RightColumnNames.Length; k++)
                    {
                        if (SQLiteParser.Utils.Chop(cname).ToLower() == 
                            SQLiteParser.Utils.Chop(RightColumnNames[k]).ToLower())
                        {
                            if (!LeftFields[i].Equals(RightFields[k]))
                            {
                                Result = ComparisonResult.DifferentData;
                                break;
                            }
                        }
                    }
                } // for
            }
            else
            {
                if (isLeft)
                    Result = ComparisonResult.ExistsInLeftDB;
                else
                    Result = ComparisonResult.ExistsInRightDB;
            } // else
        }
    }
}
