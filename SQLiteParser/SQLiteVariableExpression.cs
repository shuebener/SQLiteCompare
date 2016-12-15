using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteVariableExpression : SQLiteExpression
    {
        private SQLiteVariableExpression()
        {
        }

        public SQLiteVariableExpression(string variable)
        {
            _variable = variable;
        }

        public string Variable
        {
            get { return _variable; }
            set { _variable = value; }
        }

        public override bool IsConstant(bool allowNull)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteVariableExpression dst = obj as SQLiteVariableExpression;
            if (dst == null)
                return false;

            if (_variable != dst._variable)
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            return _variable;
        }

        public override object Clone()
        {
            SQLiteVariableExpression res = new SQLiteVariableExpression();
            res._variable = _variable;
            return res;
        }

        private string _variable;
    }
}
