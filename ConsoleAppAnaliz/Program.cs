using System;
using System.Linq;
using System.IO;
using System.Globalization;
using System.Web;

namespace ConsoleAppAnaliz
{
    class Program
    {
        static void Main(string[] args)
        {

            DateTime start = new DateTime(2022, 9, 17, 0, 0, 0, DateTimeKind.Utc);
            DateTime end = new DateTime(2022, 9, 19, 0, 0, 0, DateTimeKind.Utc);

            Console.WriteLine(HttpUtility.UrlEncode(start.ToString("O")));
            Console.WriteLine(HttpUtility.UrlEncode(end.ToString("O")));

            Console.WriteLine("press any key...");
            Console.ReadKey();
            return;

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
