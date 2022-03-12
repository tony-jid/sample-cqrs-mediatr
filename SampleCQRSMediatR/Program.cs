using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SampleCQRSMediatR.Data.Contexts.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleCQRSMediatR
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Original implementation
            //CreateHostBuilder(args).Build().Run();

            // 1. Get the IWebHost which will host this application.
            var host = CreateHostBuilder(args).Build();

            // 2. Find the service layer within our scope.
            using (var scope = host.Services.CreateScope())
            {
                // 3. Get the instance of the service provider in our services layer
                var serviceProvider = scope.ServiceProvider;

                // 4. Call the DataSeeder to initialize sample data
                DataSeeder.Seed(serviceProvider);
            }

            // 5. Continue to run the application
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
