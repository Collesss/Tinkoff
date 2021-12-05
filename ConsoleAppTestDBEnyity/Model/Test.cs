using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppTestDBEnyity.Model
{
    public class Test
    {
        public int Id { get; set; }

        public int TestValue;

        public Test()
        {
            Console.WriteLine("ctor.1");

            //val = testConstructValue;
        }
    }
}
