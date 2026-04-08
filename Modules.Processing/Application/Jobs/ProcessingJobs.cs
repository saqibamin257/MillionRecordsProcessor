using Modules.Processing.Application.Services;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Text;
using Serilog.Context;

namespace Modules.Processing.Application.Jobs
{
    public class ProcessingJobs
    {
        private readonly ProcessingService _service;

        public ProcessingJobs(ProcessingService service)
        {
            _service = service;
        }


        [AutomaticRetry(Attempts = 3)]
        [Queue("processing")]
        public async Task StartProcessing()
        {
            using (LogContext.PushProperty("JobName", "DailyProcessing"))
            {
                await _service.StartAsync();
            }
            
        }
        [AutomaticRetry(Attempts = 5)]
        [Queue("retry")]
        public async Task RetryFailed()
        {
            using (LogContext.PushProperty("JobName", "RetryFailed"))
            {
                await _service.RetryFailedAsync();
            }
        }
    }
}
