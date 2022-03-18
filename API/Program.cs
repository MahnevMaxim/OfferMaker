using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Quartz.Core;
using System.IO;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureServices((hostContext, services) =>
                {
                    //Добавление задания в планировщик.
                    // Add the required Quartz.NET services
                    services.AddQuartz(q =>
                    {
                        // Use a Scoped container to create jobs. I'll touch on this later
                        q.UseMicrosoftDependencyInjectionScopedJobFactory();

                        // Create a "key" for the job
                        var jobKey = new JobKey("CurrencyJob");

                        // Register the job with the DI container
                        q.AddJob<CurrencyJob>(opts => opts.WithIdentity(jobKey));

                        // Create a trigger for the job
                        q.AddTrigger(opts => opts
                            .ForJob(jobKey) // link to the HelloWorldJob
                            .WithIdentity("CurrencyJob-trigger") // give the trigger a unique name
                            .WithCronSchedule("0 0/5 * * * ?"));

                        // Create a "key" for the job
                        var jobKeyImages = new JobKey("ImagesJob");

                        // Register the job with the DI container
                        q.AddJob<ImagesJob>(opts => opts.WithIdentity(jobKeyImages));

                        // Create a trigger for the job
                        q.AddTrigger(opts => opts
                            .ForJob(jobKeyImages) // link to the HelloWorldJob
                            .WithIdentity("ImagesJob-trigger") // give the trigger a unique name
                            .WithCronSchedule("0 0/1 * * * ?"));
                    });

                    // Add the Quartz.NET hosted service
                    services.AddQuartzHostedService(
                        q => q.WaitForJobsToComplete = true);

                    // other config
                });
    }

    /// <summary>
    /// Задание для планировщика.
    /// </summary>
    [DisallowConcurrentExecution]
    public class CurrencyJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            CurrenciesUpdater.Update();
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Задание для планировщика.
    /// </summary>
    [DisallowConcurrentExecution]
    public class ImagesJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            ImagesUpdater.Update();
            return Task.CompletedTask;
        }
    }
}
