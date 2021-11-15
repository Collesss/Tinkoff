using System;
using System.Linq;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;
using OfficeOpenXml;
using System.IO;

namespace ConsoleAppTinkoffApiTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Test().Wait();

            //Test2().Wait();

            Console.Write("press any key...");
            Console.ReadKey();
        }


        public async static Task Test2()
        {
            // токен аутентификации
            var token = "t.F-e_MvGHyM5RydIcD28rwvIuOpgfChfOokIlqKWYOm9JKUeJFLQwlZMP0O6p_hneiDWOAjT90UQzJSlEvBZSog";
            // для работы в песочнице используйте GetSandboxConnection
            var connection = ConnectionFactory.GetConnection(token);
            var context = connection.Context;

            var list = await context.MarketStocksAsync();

            foreach (var item in list.Instruments)
            {
                Console.WriteLine($"{item.Name}: {item.Figi}: {item.Ticker};");
            }

            Console.WriteLine(list.Total);
        }

        public async static Task Test()
        {
            // токен аутентификации
            var token = "t.F-e_MvGHyM5RydIcD28rwvIuOpgfChfOokIlqKWYOm9JKUeJFLQwlZMP0O6p_hneiDWOAjT90UQzJSlEvBZSog";
            // для работы в песочнице используйте GetSandboxConnection
            var connection = ConnectionFactory.GetConnection(token);
            var context = connection.Context;

            //TinkoffCuterRequest

            int days = 100;
            /*
            Console.WriteLine(new TinkoffCuterRequest(CandleInterval.Hour, DateTime.Now - TimeSpan.FromDays(days), DateTime.Now).CountRequest);

            foreach (var FromTo in new TinkoffCuterRequest(CandleInterval.Hour, DateTime.Now - TimeSpan.FromDays(days), DateTime.Now))
            {
                Console.WriteLine($"{FromTo.from}; {FromTo.to};");
            }

            Console.WriteLine();
            */
            GetDataContext getDataContext = new GetDataContext(context, 240, TimeSpan.FromMinutes(1));

            var candles = (await getDataContext.GetData("BBG004S683W7", CandleInterval.Hour, DateTime.Now - TimeSpan.FromDays(days), DateTime.Now))
                .GroupBy(el => GetGroup(el.Time))
                .Select(group => Data.AgregateCandle(group))
                .OrderBy(aggCandle => aggCandle.OpenTime);

            /*
            var candles = new TinkoffCuterRequest(CandleInterval.Hour, DateTime.Now - TimeSpan.FromDays(days), DateTime.Now)
                .Select(async dateFromTo =>
            await context.MarketCandlesAsync("BBG000T88BN2", dateFromTo.from, dateFromTo.to, CandleInterval.Hour))
                .SelectMany(cutCandlesReq => cutCandlesReq.Result.Candles)
                .GroupBy(candle => GetGroup(candle.Time))
                .Select(group => Data.AgregateCandle(group))
                .OrderBy(DataCandle => DataCandle.OpenTime);
            
            */



            SaveInXml.Save("info.xlsx", "Sheet 1", candles, new (Func<Data, object> element, string header, string format)[]
            {
                (d => d.CloseTime, "CloseTime", "dd.MM.yyyy HH:mm"),
                (d => d.Open, "CloseTime", null),
                (d => d.Close, "CloseTime", null),
                (d => d.Low, "CloseTime", null),
                (d => d.High, "CloseTime", null)
            });
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
