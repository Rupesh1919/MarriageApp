using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarriageApp.API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MarriageApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host=CreateHostBuilder(args).Build();
            using(var scope=host.Services.CreateScope()){
                var services=scope.ServiceProvider;
                try{
                    var context=services.GetRequiredService<DataContext>();
                    context.Database.Migrate();
                    Seed.SeedUser(context);

                }catch(Exception ex){
                    var logger=services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex,"An error occured during migration");
                }
            }
            var config=new ConfigurationBuilder().AddEnvironmentVariables("").Build();
            var url= config["http://localhost:5000"]??"http://*:8080";
            
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
