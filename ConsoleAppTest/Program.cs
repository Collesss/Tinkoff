using MyLogger;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using TinkoffMyConnectionFactory;

namespace ConsoleAppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Test1().Wait();

            Console.Write("press any key...");
            Console.ReadKey();
        }

        public async static Task Test1()
        {
            // токен аутентификации
            var token = "t.F-e_MvGHyM5RydIcD28rwvIuOpgfChfOokIlqKWYOm9JKUeJFLQwlZMP0O6p_hneiDWOAjT90UQzJSlEvBZSog";
            // для работы в песочнице используйте GetSandboxConnection
            var connection = MyConnectionFactory.GetConnection(token, new MyLogger<MyContext>("log.txt"));
            var context = connection.Context;

            var list = await context.MarketStocksAsync();

            foreach (var item in list.Instruments)
            {
                Console.WriteLine($"{item.Name}: {item.Figi}: {item.Ticker};");
            }

            Console.WriteLine(list.Total);
            Console.WriteLine();

            int days = 100;

            int i = 1;

            foreach (var item in list.Instruments)
            {
                var candles = (await context.MarketCandlesAsync(item.Figi, DateTime.Now - TimeSpan.FromDays(days), DateTime.Now, CandleInterval.Hour)).Candles
                .GroupBy(el => GetGroup(el.Time))
                .Select(group => Data.AgregateCandle(group))
                .OrderBy(aggCandle => aggCandle.OpenTime);

                await SaveInXml.Save($"{item.Figi}.xlsx", "Data", candles, new (Func<Data, object> element, string header, string format)[]
                {
                    (d => d.CloseTime, "CloseTime", "dd.MM.yyyy HH:mm"),
                    (d => d.Open, "Open", null),
                    (d => d.Close, "Cloes", null),
                    (d => d.Low, "Low", null),
                    (d => d.High, "High", null)
                });

                Console.WriteLine($"{item.Figi}; {i}/{list.Total}");

                i++;
            }
        }

        static string GetGroup(DateTime dateTime)
        {
            ValueInRange<DateTime, TimeSpan, int> valueInRange = new ValueInRange<DateTime, TimeSpan, int>(
                new (TimeSpan, int)[]
                {
                    (TimeSpan.FromHours(11), 1),
                    (TimeSpan.FromHours(15), 2),
                    (TimeSpan.FromHours(19), 3),
                    (TimeSpan.FromHours(24), 4)
                },
                (value, rangeValue) => value.Hour < rangeValue.TotalHours);

            return $"{dateTime.Year}{dateTime.Month}{dateTime.Day}{valueInRange[dateTime]}";
        }
    }
}
