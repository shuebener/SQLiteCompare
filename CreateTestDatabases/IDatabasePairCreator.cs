using System;
using System.Collections.Generic;
using System.Text;

namespace CreateTestDatabases
{
    public interface IDatabasePairCreator
    {
        void CreatePair(string db1, string db2);
    }
}
