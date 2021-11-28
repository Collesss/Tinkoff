using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyLogger;
using save = MySaver;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using TinkoffMyConnectionFactory;

namespace ConsoleAppTest
{
    class Program
    {
        private static IServiceProvider Services { get; set; }

        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                .AddJsonFile($@"{Directory.GetCurrentDirectory()}\config.json")
                .AddCommandLine(args)
                .Build());


            services.AddSingleton(sp =>
                MyConnectionFactory.GetConnection(sp.GetRequiredService<IConfiguration>().GetValue<string>("token"),
                sp.GetRequiredService<ILogger<MyContext>>()));

            services.AddSingleton<save.ISave<(string fileName, string sheetName)>>(new save.SaveInXml());

            Services = services.BuildServiceProvider();

            new MyMain(Services).Main();

            Test1(args).Wait();

            Console.Write("press any key...");
            Console.ReadKey();
        }
        public async static Task Test1(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile($@"{Directory.GetCurrentDirectory()}\config.json")
                .AddCommandLine(args)
                .Build();

            // токен аутентификации
            var token = configuration.GetValue<string>("token");
            /*
            Console.WriteLine(token);
            Console.ReadKey();
            */
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

            Regex regex = new Regex(@"[\\\/:*?""<>|]");

            int days = configuration.GetValue<int>("days");

            int i = 1;

            foreach (var item in list.Instruments)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();

                    if (consoleKeyInfo.Key == ConsoleKey.C && (consoleKeyInfo.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control)
                        break;
                }

                var candles = (await context.MarketCandlesAsync(item.Figi, DateTime.Now - TimeSpan.FromDays(days), DateTime.Now, CandleInterval.Hour)).Candles
                .GroupBy(el => GetGroup(el.Time))
                .Select(group => Data.AgregateCandle(group))
                .OrderBy(aggCandle => aggCandle.OpenTime);

                await SaveInXml.Save($"{regex.Replace(item.Name, match => "_")}.xlsx", "Data", candles, new (Func<Data, object> element, string header, string format)[]
                {
                    (d => d.CloseTime, "CloseTime", "dd.MM.yyyy HH:mm"),
                    (d => d.Open, "Open", null),
                    (d => d.Close, "Cloes", null),
                    (d => d.Low, "Low", null),
                    (d => d.High, "High", null)
                });

                Console.WriteLine($"Figi:{item.Figi} Name:{item.Name}; {i}/{list.Total}");
                
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
