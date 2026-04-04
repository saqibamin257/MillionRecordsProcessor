using Hangfire;
using Modules.Processing.Application.Services;
using Processor.Worker.Jobs;

namespace Processor.Worker
{
    public class Worker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public Worker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var lastRunDate = DateTime.MinValue;
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;

                if (now.Hour == 8 && now.Minute == 0 && lastRunDate.Date != now.Date)
                {
                    BackgroundJob.Enqueue<ProcessingJobs>(x => x.StartProcessing());

                    lastRunDate = now;    
                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        //protected override Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    RecurringJob.AddOrUpdate<ProcessingJobs>(
        //        "daily-processing-job",
        //        job => job.StartProcessing(),
        //        "0 8 * * *"
        //    );

        //    return Task.CompletedTask;
        //}
    }
}
