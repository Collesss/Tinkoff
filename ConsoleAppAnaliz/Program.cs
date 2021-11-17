using System;
using System.Linq;
using System.IO;
using System.Globalization;

namespace ConsoleAppAnaliz
{
    class Program
    {
        static void Main(string[] args)
        {
            using (StreamReader reader = new StreamReader(new FileStream("log.txt", FileMode.Open, FileAccess.Read)))
            {
                var groups = reader.ReadToEnd().Split('\n').Select(ts => DateTime.Parse(ts, new CultureInfo("ru-RU"))).GroupBy(dt => $"{dt.Hour} {dt.Minute}");

                foreach (var group in groups)
                {
                    Console.WriteLine($"group.Key|{group.Count()}");
                }

            }

            Console.WriteLine("press any key...");
            Console.ReadKey();
        }
    }
}
