using Microsoft.Extensions.Configuration;
using MySaver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;

namespace ConsoleAppTest
{
    public class MyMain
    {
        private readonly IConfiguration _configuration;
        private readonly ISave<(string fileName, string sheetName)> _save;
        private readonly IConnection<IContext> _connection;
        private readonly TextWriter _streamWriter;
        private readonly int _days;

        public MyMain(IConnection<IContext> connection, ISave<(string fileName, string sheetName)> save, TextWriter streamWriter, int days)
        {
            _save = save;
            _connection = connection;
            _streamWriter = streamWriter;
            _days = days;
        }

        public async Task Main(CancellationToken cancellationToken)
        {
            IContext context = _connection.Context;

            var list = await context.MarketStocksAsync();
            
            foreach (var item in list.Instruments)
            {
                _streamWriter.WriteLine($"{item.Name}: {item.Figi}: {item.Ticker};");
            }

            _streamWriter.WriteLine(list.Total);
            _streamWriter.WriteLine();

            Regex regex = new Regex(@"[\\\/:*?""<>|]");

            int i = 1;

            foreach (var item in list.Instruments)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                /*
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();

                    if (consoleKeyInfo.Key == ConsoleKey.C && (consoleKeyInfo.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control)
                        break;
                }
                */

                var candles = (await context.MarketCandlesAsync(item.Figi, DateTime.Now - TimeSpan.FromDays(_days), DateTime.Now, CandleInterval.Hour)).Candles
                    .GroupBy(el => $"{el.Time.Year}{el.Time.Month}{el.Time.Day}{(el.Time.Hour + 1) / 4}")
                    .Select(group => Data.AgregateCandle(group))
                    .OrderBy(aggCandle => aggCandle.OpenTime);

                await _save.Save(($"{regex.Replace(item.Name, match => "_")}.xlsx", "Data"), candles, new (Func<Data, object> element, string header, string format)[]
                {
                    (d => d.CloseTime, "CloseTime", "dd.MM.yyyy HH:mm"),
                    (d => d.Open, "Open", null),
                    (d => d.Close, "Cloes", null),
                    (d => d.Low, "Low", null),
                    (d => d.High, "High", null)
                });

                _streamWriter.WriteLine($"Figi:{item.Figi} Name:{item.Name}; {i}/{list.Total}");

                i++;
            }
        }
    }
}
