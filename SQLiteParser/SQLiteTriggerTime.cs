using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public enum SQLiteTriggerTime
    {
        None = 0,

        Before = 1,

        After = 2,

        InsteadOf = 3,
    }
}
