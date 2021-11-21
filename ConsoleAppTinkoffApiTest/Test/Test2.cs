using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppTinkoffApiTest.Test
{
    public class Test2 : Test1, ITest
    {
        public new string GetStr() => "Test2";
    }
}
