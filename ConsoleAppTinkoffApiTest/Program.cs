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


            int i = 2;

            if (File.Exists("info.xlsx"))
                File.Delete("info.xlsx");

            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo("info.xlsx")))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");

                worksheet.Cells["A:B"].Style.Numberformat.Format = "dd.MM.yyyy HH:mm";

                worksheet.Cells[1, 1].Value = "Open Time";
                worksheet.Cells[1, 2].Value = "Close Time";
                worksheet.Cells[1, 3].Value = "Open";
                worksheet.Cells[1, 4].Value = "Close";
                worksheet.Cells[1, 5].Value = "Low";
                worksheet.Cells[1, 6].Value = "High";

                foreach (var candle in candles)
                {
                    Console.WriteLine($"{candle.OpenTime}; {candle.CloseTime}; {candle.Open}; {candle.Close}; {candle.Low}; {candle.High}; {candle.Volume}");

                    worksheet.Cells[i, 1].Value = candle.OpenTime;//.ToString("dd.MM.yyyy H:mm");
                    worksheet.Cells[i, 2].Value = candle.CloseTime;
                    worksheet.Cells[i, 3].Value = candle.Open;
                    worksheet.Cells[i, 4].Value = candle.Close;
                    worksheet.Cells[i, 5].Value = candle.Low;
                    worksheet.Cells[i, 6].Value = candle.High;

                    i++;
                }

                worksheet.Cells["A:B"].AutoFitColumns();

                excelPackage.Save();
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
