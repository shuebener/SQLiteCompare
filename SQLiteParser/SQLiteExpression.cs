using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteExpression
    {
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteExpression dst = obj as SQLiteExpression;
            if (dst == null)
                return false;
            return true;
        }

        public override string ToString()
        {
            throw new InvalidOperationException("SQL statement rendering has failed");
        }

        public virtual object Clone()
        {
            throw new InvalidOperationException("SQL statement cloning has failed");
        }

        public virtual bool IsConstant(bool allowNull)
        {
            throw new NotSupportedException("IsConstant is not supported in the base class");
        }
    }
}
