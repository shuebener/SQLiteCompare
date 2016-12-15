using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public enum SQLiteOperator
    {
        None = 0,

        And = 1,

        Or = 2,

        Lt = 3,

        Gt = 4,

        Ge = 5,

        Le = 6,

        Eq = 7,

        Ne = 8,

        BitAnd = 9,

        BitOr = 10,

        Lshift = 11,

        Rshift = 12,

        Plus = 13,

        Minus = 14,

        Star = 15,

        Slash = 16,

        Rem = 17,

        Concat = 18,

        IsNull = 19,

        NotNull = 20,

        Is_Null = 21,

        Not_Null = 22,

        Is_Not_Null = 23,

        Not = 24,

        BitNot = 25,
    }
}
