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
using Microsoft.Extensions.Options;

namespace ConsoleAppTest
{
    class Program
    {
        private static IServiceProvider Services { get; set; }
        private static IConfiguration Configuration { get; set; }

        static void Main(string[] args)
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile($@"{Directory.GetCurrentDirectory()}\config.json", true)
                .AddCommandLine(args)
                .Build();

            Services = new ServiceCollection()
                .AddSingleton(Configuration)
                .Configure<Options>(Configuration.GetSection("Options"))
                .AddSingleton<ILogger<MyContext>>(sp =>
                    new MyLoggerFile<MyContext>(sp.GetRequiredService<IOptions<Options>>().Value.LogFile))
                .AddSingleton<ILogger<MyMain>, MyLoggerConsole<MyMain>>()
                .AddSingleton(sp =>
                    MyConnectionFactory.GetConnection(
                        sp.GetRequiredService<IOptions<Options>>().Value.Token,
                        sp.GetRequiredService<ILogger<MyContext>>()))
                .AddSingleton<save.ISave<(string fileName, string sheetName)>, save.SaveInXml>()
                .AddSingleton<MyMain>()
                .AddDbContext<DBTinkoffContext>((services, options) =>
                    options.UseSqlite(services.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection")))
                .AddScoped<IRepository<EntityCandlePayload>, Repository<DBTinkoffContext, EntityCandlePayload>>()
                .AddScoped<IRepository<EntityMarketInstrument>, Repository<DBTinkoffContext, EntityMarketInstrument>>()
                .AddScoped<IRepository<EntityDataAboutAlreadyLoaded>, Repository<DBTinkoffContext, EntityDataAboutAlreadyLoaded>>()
                .AddSingleton<ICustomFilter>(sp => new CustomFilter(sp.GetRequiredService<IOptions<Options>>().Value.CustomFilterData))
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
