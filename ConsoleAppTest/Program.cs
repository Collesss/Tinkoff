using DBTinkoff;
using DBTinkoff.Repositories;
using DBTinkoffEntities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyLogger;
using System;
using System.IO;
using System.Threading;
using TinkoffMyConnectionFactory;
using save = MySaver;

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

            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();

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

        private static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton(Configuration)
                .Configure<Options>(Configuration.GetSection("Options"))
                .AddSingleton<ILogger<MyContext>>(sp =>
                    new MyLoggerFile<MyContext>(sp.GetRequiredService<IOptions<Options>>().Value.LogFile))
                .AddSingleton<ILogger<MyMain>, MyLoggerConsole<MyMain>>()
                .AddSingleton(sp =>
                    MyConnectionFactory.GetConnection(
                        sp.GetRequiredService<IOptions<Options>>().Value.Token,
                        sp.GetRequiredService<ILogger<MyContext>>()))
                .AddSingleton<save.ISave<(string fileName, string sheetName), Data>>(sp =>
                    new save.SaveInExcel<Data>(new (Func<Data, object> element, string header, string format)[]
                    {
                        (d => d.CloseTime, "CloseTime", "dd.MM.yyyy HH:mm"),
                        (d => d.Open, "Open", null),
                        (d => d.Close, "Close", null),
                        (d => d.Low, "Low", null),
                        (d => d.High, "High", null)
                    }, "Data"))
                .AddSingleton<MyMain>()
                .AddDbContext<DBTinkoffContext>((services, options) =>
                    options.UseSqlite(services.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection")))
                .AddScoped<IRepository<EntityCandlePayload>, Repository<DBTinkoffContext, EntityCandlePayload>>()
                .AddScoped<IRepository<EntityMarketInstrument>, Repository<DBTinkoffContext, EntityMarketInstrument>>()
                .AddScoped<IRepository<EntityDataAboutAlreadyLoaded>, Repository<DBTinkoffContext, EntityDataAboutAlreadyLoaded>>()
                .AddSingleton<ICustomFilter>(sp => new CustomFilter(sp.GetRequiredService<IOptions<Options>>().Value.CustomFilterData));
        }
    }
}
