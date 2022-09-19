using System;
using Tinkoff.Trading.OpenApi.Models;

namespace ConsoleAppTest
{
    public class Options
    {
        public string Token { get; set; }
        public string LogFile { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public bool UseCustomSave { get; set; }
        public bool SkipSaturdaySunday { get; set; }
        public bool CustomFilter { get; set; }
        public string CustomFilterData { get; set; }
    }
}
