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
using System.Threading;
using Tinkoff.Trading.OpenApi.Network;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using DBTinkoff;
using DBTinkoff.Repositories;
using DBTinkoffEntities.Entities;

namespace ConsoleAppTest
{
    class Program
    {
        private static IServiceProvider Services { get; set; }

        static void Main(string[] args)
        {
            Services = new ServiceCollection()
                .AddSingleton<IConfiguration>(new ConfigurationBuilder()
                    .AddJsonFile($@"{Directory.GetCurrentDirectory()}\config.json")
                    .AddCommandLine(args)
                    .Build())
                .AddSingleton<ILogger<MyContext>>(sp =>
                    new MyLoggerFile<MyContext>(sp.GetRequiredService<IConfiguration>().GetValue<string>("logFile")))
                .AddSingleton<ILogger<MyMain>, MyLoggerConsole<MyMain>>()
                .AddSingleton(sp =>
                    MyConnectionFactory.GetConnection(
                        sp.GetRequiredService<IConfiguration>().GetValue<string>("token"),
                        sp.GetRequiredService<ILogger<MyContext>>()))
                .AddSingleton<save.ISave<(string fileName, string sheetName)>, save.SaveInXml>()
                .AddSingleton(sp => 
                    new MyMain(
                        sp,
                        sp.GetRequiredService<IConnection<IContext>>(),
                        sp.GetRequiredService<IRepository<EntityCandlePayload>>(),
                        sp.GetRequiredService<IRepository<EntityMarketInstrument>>(),
                        sp.GetRequiredService<save.ISave<(string fileName, string sheetName)>>(),
                        sp.GetRequiredService<ILogger<MyMain>>(),
                        sp.GetRequiredService<IConfiguration>().GetValue<int>("days")))
                .AddDbContext<DBTinkoffContext>((services, options) =>
                    options.UseSqlite(services.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection")))
                .AddScoped<IRepository<EntityCandlePayload>, Repository<DBTinkoffContext, EntityCandlePayload>>()
                .AddScoped<IRepository<EntityMarketInstrument>, Repository<DBTinkoffContext, EntityMarketInstrument>>()
                .BuildServiceProvider();

            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

            Services.GetRequiredService<MyMain>().Main(cancelTokenSource.Token).Wait();

            /*
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();

                if (consoleKeyInfo.Key == ConsoleKey.C && (consoleKeyInfo.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control)
                    cancelTokenSource.Cancel();
            }
            */
            //Test1(args).Wait();

            Console.Write("press any key...");
            Console.ReadKey();
        }
    }
}
