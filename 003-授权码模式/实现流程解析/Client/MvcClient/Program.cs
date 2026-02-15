using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace MvcClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //List<long> longlist = new List<long>();
            //for (int i = 0; i < 1000; i++)
            //{
            //    longlist.Add(SnowflakeHelper.Next());
            //}

            //longlist= longlist.Distinct().ToList();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("https://localhost:7001");
                });
    }
}
