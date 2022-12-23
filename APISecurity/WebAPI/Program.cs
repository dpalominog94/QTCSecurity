using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI
{
    public class Program
    {
        /**TODO: Log Fisico*/
        //private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ILogger<Program> logger;
        public Program()
        {
            var services = new ServiceCollection()
                .AddLogging(logBuilder => logBuilder.SetMinimumLevel(LogLevel.Debug))
                .BuildServiceProvider();


            //logger = services.GetService<ILoggerFactory>()
            //    .AddLog4Net()
            //    .CreateLogger<Program>();
        }

        /**                */
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
