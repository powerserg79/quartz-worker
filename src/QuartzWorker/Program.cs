using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using QuartzWorker.Quartz;
using QuartzWorker.Quartz.Jobs;

namespace QuartzWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IJobFactory, JobFactory>();
                    services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();                    
                    //
                    #region Adding JobType
                    services.AddSingleton<NotificationJob>();
                    services.AddSingleton<LoggerJob>();
                    #endregion
                    
                    #region Adding Jobs 
                    List<JobMeta> jobMetadatas = new List<JobMeta>();
                    jobMetadatas.Add(new JobMeta(Guid.NewGuid(), typeof(NotificationJob), "Notify Job", "0/10 * * * * ?"));
                    jobMetadatas.Add(new JobMeta(Guid.NewGuid(), typeof(LoggerJob), "Log Job", "0/5 * * * * ?"));
                    
                    services.AddSingleton(jobMetadatas);
                    #endregion

                    services.AddHostedService<DefaultSchedular>();
                    
                    
                    // services.AddHostedService<Worker>();
                });
    }
}