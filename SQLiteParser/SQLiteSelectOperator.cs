using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    /// <summary>
    /// Describes all SELECT operators
    /// </summary>
    public enum SQLiteSelectOperator
    {
        None = 0,

        Union = 1,

        UnionAll = 2,

        Except = 3,

        Intersect = 4,
    }
}
