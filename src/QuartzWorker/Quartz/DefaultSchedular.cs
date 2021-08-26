using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using QuartzWorker.Quartz;

namespace QuartzWorker
{
   
    class DefaultSchedular : IHostedService
    {
        public IScheduler Scheduler { get; set; }
        private readonly IJobFactory jobFactory;
        private readonly List<JobMeta> _jobMetas;
        private readonly ISchedulerFactory schedulerFactory;

        public DefaultSchedular(ISchedulerFactory schedulerFactory,List<JobMeta> jobMetas,IJobFactory jobFactory)
        {
            this.jobFactory = jobFactory;
            this.schedulerFactory = schedulerFactory;
            this._jobMetas = jobMetas;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //Creating Schdeular
            Scheduler = await schedulerFactory.GetScheduler();
            Scheduler.JobFactory = jobFactory;

            //Suporrt for Multiple Jobs
            _jobMetas?.ForEach(jobMetadata =>
            {
                //Create Job
                IJobDetail jobDetail = CreateJob(jobMetadata);
                
                //Create trigger
                ITrigger trigger = CreateTrigger(jobMetadata);
                
                //Schedule Job
                Scheduler.ScheduleJob(jobDetail, trigger, cancellationToken).GetAwaiter();
           
            });
            
            //Start The Schedular
            await Scheduler.Start(cancellationToken);
        }

        private ITrigger CreateTrigger(JobMeta jobMetadata)
        {
            return TriggerBuilder.Create()
                .WithIdentity(jobMetadata.JobId.ToString())
                .WithCronSchedule(jobMetadata.CronExpression)
                .WithDescription(jobMetadata.JobName)
                .Build();
        }

        private IJobDetail CreateJob(JobMeta jobMetadata)
        {
            return JobBuilder.Create(jobMetadata.JobType)
                .WithIdentity(jobMetadata.JobId.ToString())
                .WithDescription(jobMetadata.JobName)
                .Build();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Scheduler.Shutdown();
        }
    }
}