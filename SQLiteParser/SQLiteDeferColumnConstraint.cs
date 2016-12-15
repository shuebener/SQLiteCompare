using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteDeferColumnConstraint : SQLiteColumnConstraint
    {
        private SQLiteDeferColumnConstraint(string name)
            : base(name)
        {
        }

        public SQLiteDeferColumnConstraint(string name, bool deferrable, SQLiteDeferType deferType)
            : base(name)
        {
            _deferType = deferType;
            _deferrable = deferrable;
        }

        public bool IsDeferrable
        {
            get { return _deferrable; }
            set { _deferrable = value; }
        }

        public SQLiteDeferType DeferType
        {
            get { return _deferType; }
            set { _deferType = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteDeferColumnConstraint dst = obj as SQLiteDeferColumnConstraint;
            if (dst == null)
                return false;

            if (this._deferrable != dst._deferrable)
                return false;
            if (this._deferType != dst._deferType)
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (_deferrable)
                sb.Append("DEFERRABLE");
            else
                sb.Append("NOT DEFERRABLE");
            if (_deferType == SQLiteDeferType.InitiallyDeferred)
                sb.Append(" INITIALLY DEFERRED");
            else if (_deferType == SQLiteDeferType.InitiallyImmediate)
                sb.Append(" INITIALLY IMMEDIATE");
            return sb.ToString();
        }

        public override object Clone()
        {
            SQLiteDeferColumnConstraint res = new SQLiteDeferColumnConstraint(this.ConstraintName);
            res._deferType = _deferType;
            res._deferrable = _deferrable;
            return res;
        }

        private bool _deferrable;
        private SQLiteDeferType _deferType;
    }

    public enum SQLiteDeferType
    {
        None = 0,

        InitiallyDeferred = 1,

        InitiallyImmediate = 2,
    }
}
