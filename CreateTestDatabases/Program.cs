using System;
using System.Collections.Generic;
using System.Text;

namespace CreateTestDatabases
{
    class Program
    {
        static IDatabasePairCreator[] _checks = new IDatabasePairCreator[] 
            {
                new CreateDataChecks_1(),
                new CreateDataChecks_2(),
                new CreateDataChecks_3(),
                new CreateTriggerChecks_1(),
            };

        static void Main(string[] args)
        {
            for (int i = 0; i < _checks.Length; i++)
            {
                // Mickey Mouse
                _checks[i].CreatePair("test_left_" + i + ".db", "test_right_" + i + ".db");
            } // for
        }
    }
}
