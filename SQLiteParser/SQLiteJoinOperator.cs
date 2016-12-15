using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    [Flags]
    public enum SQLiteJoinOperator
    {
        Comma       = 0x01,

        Natural     = 0x02,

        Left        = 0x04,

        Inner       = 0x08,

        Cross       = 0x10,

        Outer       = 0x20,

        Join        = 0x40,
    }
}
