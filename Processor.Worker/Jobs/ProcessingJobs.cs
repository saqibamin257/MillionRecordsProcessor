using Modules.Processing.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Processor.Worker.Jobs
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
    }
}
