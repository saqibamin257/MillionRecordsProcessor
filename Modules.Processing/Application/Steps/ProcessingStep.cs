using Modules.Processing.Application.Context;
using Modules.Processing.Application.Pipeline;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Processing.Application.Steps
{
    public class ProcessingStep : IProcessingStep
    {
        public async Task ExecuteAsync(RecordContext context, CancellationToken ct)
        {
            try
            {
                await Task.Delay(10, ct);

                if (Random.Shared.Next(1, 10) == 5)
                    throw new Exception("Random failure");

                context.Record.MarkProcessed();
            }
            catch (Exception ex)
            {
                context.Record.MarkFailed();
                context.Fail(ex.Message);
            }
        }
    }
}
