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

namespace API
{
    public class Program
    {
        static IScheduler scheduler;

        public static void Main(string[] args)
        {
            //StartScheduler();
            CreateHostBuilder(args).Build().Run();
            //StopScheduler();
        }

        async private static void AddJob()
        {
            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<CurrencyJob>()
                .WithIdentity("job1", "group1")
                .Build();

            // Trigger the job to run now, and then repeat every 10 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(5)
                    .RepeatForever())
                .Build();

            // Tell quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);
        }

        async private static void StopScheduler() => await scheduler.Shutdown();

        async private static void StartScheduler()
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            scheduler = await factory.GetScheduler();
            await scheduler.Start();
            AddJob();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    public class CurrencyJob : IJob
    {
        static bool isBusy;
        public async Task Execute(IJobExecutionContext context)
        {
            if (isBusy) return;
            isBusy = true;
            CurrenciesUpdater.Update();
        }
    }
}
