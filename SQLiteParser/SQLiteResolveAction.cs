using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    /// <summary>
    /// Lists possible conflict resolve actions
    /// </summary>
    public enum SQLiteResolveAction
    {
        None = 0,

        Ignore = 1,

        Replace = 2,

        Rollback = 3,

        Abort = 4,

        Fail = 5,
    }
}
