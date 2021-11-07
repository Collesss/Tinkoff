using System;
using System.Linq;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;

namespace ConsoleAppTinkoffApiTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Test().Wait();

            Console.Write("press any key...");
            Console.ReadKey();
        }


        public async static Task Test()
        {
            // токен аутентификации
            var token = "t.F-e_MvGHyM5RydIcD28rwvIuOpgfChfOokIlqKWYOm9JKUeJFLQwlZMP0O6p_hneiDWOAjT90UQzJSlEvBZSog";
            // для работы в песочнице используйте GetSandboxConnection
            var connection = ConnectionFactory.GetConnection(token);
            var context = connection.Context;

            //TinkoffCuterRequest

            int days = 2000;

            Console.WriteLine(new TinkoffCuterRequest(CandleInterval.Hour, DateTime.Now - TimeSpan.FromDays(days), DateTime.Now).CountRequest);

            foreach (var FromTo in new TinkoffCuterRequest(CandleInterval.Hour, DateTime.Now - TimeSpan.FromDays(days), DateTime.Now))
            {
                Console.WriteLine($"{FromTo.from}; {FromTo.to};");
            }

            Console.WriteLine();

            GetDataContext getDataContext = new GetDataContext(context, 240, TimeSpan.FromMinutes(1));

            var candles = getDataContext.GetData("BBG000T88BN2", CandleInterval.Hour, DateTime.Now - TimeSpan.FromDays(days), DateTime.Now);

            /*
            var candles = new TinkoffCuterRequest(CandleInterval.Hour, DateTime.Now - TimeSpan.FromDays(days), DateTime.Now)
                .Select(async dateFromTo =>
            await context.MarketCandlesAsync("BBG000T88BN2", dateFromTo.from, dateFromTo.to, CandleInterval.Hour))
                .SelectMany(cutCandlesReq => cutCandlesReq.Result.Candles)
                .GroupBy(candle => GetGroup(candle.Time))
                .Select(group => Data.AgregateCandle(group))
                .OrderBy(DataCandle => DataCandle.OpenTime);
            
            */
            foreach (var candle in candles)
            {
                Console.WriteLine($"{candle.Time}; {candle.Open}; {candle.Close}; {candle.Low}; {candle.High}; {candle.Volume}");
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
                    (TimeSpan.FromHours(23), 4)
                },
                (value, rangeValue) => value.Hour < rangeValue.TotalHours);

            return $"{dateTime.Year}{dateTime.Month}{dateTime.Day}{valueInRange[dateTime]}";
        }
    }
}
