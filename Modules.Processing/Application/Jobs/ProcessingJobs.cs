using Modules.Processing.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Processing.Application.Jobs
{
    public class ProcessingJobs
    {
        private readonly ProcessingService _service;

        public ProcessingJobs(ProcessingService service)
        {
            _service = service;
        }

        public async Task StartProcessing()
        {
            await _service.StartAsync();
        }
        public async Task RetryFailed()
        {
            await _service.RetryFailedAsync();
        }
    }
}
