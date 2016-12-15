using System;
using System.Collections.Generic;
using System.Text;
using SQLiteParser;

namespace SQLiteTurbo
{
    /// <summary>
    /// This class encapsulates the comparison information for a single DB object
    /// </summary>
    public class SchemaComparisonItem
    {
        #region Constructors
        /// <summary>
        /// Create a new comparison item with the name of the compared DB object, its schema
        /// object type, and the result of the comparison process.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="res">The result of the comparison process.</param>
        public SchemaComparisonItem(string name, SQLiteDdlStatement left, SQLiteDdlStatement right, ComparisonResult res)
        {
            _name = SQLiteParser.Utils.Chop(name);
            _left = left;
            _right = right;
            _result = res;
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Get/Set the name of the DB entity that was compared.
        /// </summary>
        public string ObjectName
        {
            get { return _name; }
            set { _name = SQLiteParser.Utils.Chop(value); }
        }

        /// <summary>
        /// The DDL SQL statement used to create the DB object in the left database
        /// </summary>
        public SQLiteDdlStatement LeftDdlStatement
        {
            get { return _left; }
            set { _left = value; }
        }

        /// <summary>
        /// The DDL SQL statement used to create the DB object in the right database
        /// </summary>
        public SQLiteDdlStatement RightDdlStatement
        {
            get { return _right; }
            set { _right = value; }
        }

        /// <summary>
        /// The comparison result 
        /// </summary>
        public ComparisonResult Result
        {
            get { return _result; }
            set { _result = value; }
        }

        public string ErrorMessage
        {
            get { return _error; }
            set { _error = value; }
        }

        /// <summary>
        /// Relevant only for tables. If not null - contains the result
        /// of comparing the data rows of two tables.
        /// </summary>
        public TableChanges TableChanges
        {
            get { return _tableChanges; }
            set { _tableChanges = value; }
        }
        #endregion

        #region Private Variables
        private string _name;
        private SQLiteDdlStatement _left;
        private SQLiteDdlStatement _right;
        private ComparisonResult _result;
        private TableChanges _tableChanges;
        private string _error;
        #endregion
    }

    /// <summary>
    /// Lists the possible comparison result for a database object
    /// </summary>
    public enum ComparisonResult
    {
        /// <summary>
        /// Illegal value
        /// </summary>
        None = 0,

        /// <summary>
        /// The object exists only in the left DB
        /// </summary>
        ExistsInLeftDB = 1,

        /// <summary>
        /// The object exists only in the right DB
        /// </summary>
        ExistsInRightDB = 2,

        /// <summary>
        /// The object exists in both databases but has different schemas
        /// </summary>
        DifferentSchema = 3,

        /// <summary>
        /// The object exists in both databases and has the same schema but different
        /// data (refers only to tables).
        /// </summary>
        DifferentData = 4,        

        /// <summary>
        /// The object exists in both databases and is the same.
        /// </summary>
        Same = 5,

        /// <summary>
        /// The object was deleted from both databases
        /// </summary>
        Deleted = 6,
    }

    /// <summary>
    /// Lists the possible schema object types
    /// </summary>
    public enum SchemaObject
    {
        /// <summary>
        /// Illegal value
        /// </summary>
        None = 0,

        /// <summary>
        /// A table schema object
        /// </summary>
        Table = 1,

        /// <summary>
        /// An index schema object
        /// </summary>
        Index = 2,

        /// <summary>
        /// A view schema object
        /// </summary>
        View = 3,

        /// <summary>
        /// A trigger schema object
        /// </summary>
        Trigger = 4,
    }
}
