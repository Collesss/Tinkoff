using DBTinkoff;
using DBTinkoff.Repositories.Implementations;
using DBTinkoff.Repositories.Interfaces;
using Filter;
using ExtensionPlugins;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Tinkoff.Trading.OpenApi.Models;
using TinkoffMyConnectionFactory;
using Filter.Union;
using Save;
using Save.SaveExcel;

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

            
            using (IServiceScope scope = Services.CreateScope())
            {
                CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

                scope.ServiceProvider.GetRequiredService<MyMain>().Main(cancelTokenSource.Token).Wait();
            }
            


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
                .AddSingleton<ISaveCandleMarketInstrument, SaveInExcel>()
                .AddScoped<MyMain>()
                .AddDbContext<DBTinkoffContext>((services, options) =>
                    options.UseSqlite(services.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection"))
                           .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking))
                .AddScoped<IRepositoryMarketInstrument, RepositoryMarketInstrument>()
                .AddScoped<IRepositoryCandlePayload, RepositoryCandlePayload>()
                .AddScoped<IRepositoryDataAboutAlreadyLoaded, RepositoryDataAboutAlreadyLoaded>()
                .AddSingleton<IFilterUnion, FilterUnion>()
                .AddUsePlugin<IFilter>(Configuration.GetSection("Plugins:Filters").Get<OptionsPlugins>());

        }
    }
}
