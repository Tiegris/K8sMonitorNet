using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;

namespace K8sMonitorCore;

public class Program
{
    public static void Main(string[] args) {
        Console.WriteLine("Started Sleeping");
        Thread.Sleep(new TimeSpan(0, 0, 45));
        Console.WriteLine("Sleeping ended, starting app");
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => {
                webBuilder.UseStartup<Startup>();
            });
}
