using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteTurbo
{
    /// <summary>
    /// Encapsulates all the necessary comparison parameters.
    /// </summary>
    public class CompareParams
    {
        #region Constructors
        /// <summary>
        /// Constructs comparison parameters object
        /// </summary>
        /// <param name="left">The path to the left DB file</param>
        /// <param name="right">The path to the right DB file</param>
        /// <param name="ctype">Associated comparison type</param>
        public CompareParams(string left, string right, ComparisonType ctype, bool compareBlobFields)
        {
            _left = left;
            _right = right;
            _ctype = ctype;
            _compareBlobFields = compareBlobFields;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Path to the left DB file to compare
        /// </summary>
        public string LeftDbPath
        {
            get { return _left; }
        }

        /// <summary>
        /// Path to right DB file to compare
        /// </summary>
        public string RightDbPath
        {
            get { return _right; }
        }

        /// <summary>
        /// The comparision type to use
        /// </summary>
        public ComparisonType ComparisonType
        {
            get { return _ctype; }
            set { _ctype = value; }
        }

        /// <summary>
        /// TRUE means that the compare worker will also compare BLOB field values
        /// </summary>
        public bool IsCompareBlobFields
        {
            get { return _compareBlobFields; }
            set { _compareBlobFields = value; }
        }
        #endregion

        #region Private Variables
        private string _left;
        private string _right;
        private ComparisonType _ctype = ComparisonType.None;
        private bool _compareBlobFields;
        #endregion
    }

    /// <summary>
    /// Provides various options to the comparison process.
    /// </summary>
    public enum ComparisonType
    {
        /// <summary>
        /// Illegal value
        /// </summary>
        None = 0,

        /// <summary>
        /// Compare the schema only
        /// </summary>
        CompareSchemaOnly = 1,

        /// <summary>
        /// Compare both schema and data stored in DB tables.
        /// </summary>
        CompareSchemaAndData = 2,
    }
}
