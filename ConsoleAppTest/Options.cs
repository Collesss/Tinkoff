using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppTest
{
    public class Options
    {
        public string Token { get; set; }
        public string LogFile { get; set; }
        public int Days { get; set; }
        public bool CustomFilter { get; set; }
        public string CustomFilterData { get; set; }
    }
}
