using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
using TinkoffMyConnectionFactory;

namespace ConsoleAppTest
{
    public class MyMain
    {
        private readonly ISave<(string fileName, string sheetName)> _save;
        private readonly IConnection<IContext> _connection;
        private readonly List<ILogger<MyContext>> _logger;
        private readonly int _days;

        public MyMain(IConnection<IContext> connection, ISave<(string fileName, string sheetName)> save, IEnumerable<ILogger<MyContext>> logger, int days)
        {
            _save = save;
            _connection = connection;
            _logger = logger.ToList();
            _days = days;
        }

        public async Task Main(CancellationToken cancellationToken)
        {
            IContext context = _connection.Context;

            var list = await context.MarketStocksAsync();
            
            foreach (var item in list.Instruments)
            {
                _logger.ForEach(logger => logger.LogInformation($"{item.Name}: {item.Figi}: {item.Ticker};"));
            }

            _logger.ForEach(logger => logger.LogInformation(list.Total.ToString()));

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

                _logger.ForEach(logger => logger.LogInformation($"Figi:{item.Figi} Name:{item.Name}; {i}/{list.Total}"));

                i++;
            }
        }
    }
}
