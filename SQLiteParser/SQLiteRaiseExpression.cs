using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteRaiseExpression : SQLiteExpression
    {
        public SQLiteRaiseExpression()
        {
            RaiseType = SQLiteResolveAction.Ignore;
        }

        public SQLiteRaiseExpression(SQLiteResolveAction raiseType, string errmsg)
        {
            _errmsg = errmsg;
            RaiseType = raiseType;
        }

        public SQLiteResolveAction RaiseType
        {
            get { return _raiseType; }
            set
            {
                if (value == SQLiteResolveAction.Rollback ||
                    value == SQLiteResolveAction.Abort ||
                    value == SQLiteResolveAction.Fail ||
                    value == SQLiteResolveAction.Ignore)
                    _raiseType = value;
                if (value == SQLiteResolveAction.Ignore)
                    _errmsg = null;
            }
        }

        public string ErrorMessage
        {
            get { return _errmsg; }
            set { _errmsg = value; }
        }

        public override bool IsConstant(bool allowNull)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteRaiseExpression dst = obj as SQLiteRaiseExpression;
            if (dst == null)
                return false;

            if (_raiseType != dst._raiseType || _errmsg != dst._errmsg)
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            string res;
            if (_raiseType == SQLiteResolveAction.Rollback || _raiseType == SQLiteResolveAction.Abort ||
                _raiseType == SQLiteResolveAction.Fail)
            {
                // RAISE LP raisetype COMMA nm RP
                res = "RAISE (" + Utils.GetResolveActionString(_raiseType) + "," + _errmsg + ")";
            }
            else if (_raiseType == SQLiteResolveAction.Ignore)
            {
                // RAISE LP IGNORE RP
                res = "RAISE (IGNORE)";
            }
            else
                throw new ArgumentException("Illegal raise type [" + _raiseType.ToString() + "]");

            return res;
        }

        public override object Clone()
        {
            SQLiteRaiseExpression res = new SQLiteRaiseExpression();
            res._raiseType = _raiseType;
            res._errmsg = _errmsg;
            return res;
        }

        private SQLiteResolveAction _raiseType;
        private string _errmsg;
    }
}
