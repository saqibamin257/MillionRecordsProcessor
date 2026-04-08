using Modules.Processing.Application.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Processing.Application.Pipeline
{
    public class ProcessingPipeline
    {
        private readonly IEnumerable<IProcessingStep> _steps;

        public ProcessingPipeline(IEnumerable<IProcessingStep> steps)
        {
            _steps = steps;
        }

        public async Task ExecuteAsync(RecordContext context, CancellationToken ct)
        {
            foreach (var step in _steps)
            {
                await step.ExecuteAsync(context, ct);

                if (context.HasFailed)
                    break;
            }
        }
    }
}
