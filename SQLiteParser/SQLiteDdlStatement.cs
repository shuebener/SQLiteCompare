using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    /// <summary>
    /// Serves as the base class for all SQL DDL statements
    /// </summary>
    public class SQLiteDdlStatement : SQLiteStatement
    {
        #region Constructors
        public SQLiteDdlStatement()
        {
        }

        public SQLiteDdlStatement(SQLiteObjectName name)
        {
            _objectName = name;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Get/Set the name of the SQL object
        /// </summary>
        public SQLiteObjectName ObjectName
        {
            get { return _objectName; }
            set { _objectName = value; }
        }
        #endregion

        #region Public Overrided Methods
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteDdlStatement dst = obj as SQLiteDdlStatement;
            if (dst == null)
                return false;

            return RefCompare.Compare(_objectName, dst._objectName);
        }

        public override object Clone()
        {
            SQLiteObjectName objectName = null;
            if (_objectName != null)
                objectName = (SQLiteObjectName)_objectName.Clone();

            SQLiteDdlStatement res = new SQLiteDdlStatement();
            res._objectName = objectName;
            return res;
        }
        #endregion

        #region Private Variables
        private SQLiteObjectName _objectName;
        #endregion
    }
}
