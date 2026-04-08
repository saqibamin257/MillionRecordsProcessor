using Modules.Processing.Application.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Processing.Application.Pipeline
{
    public interface IProcessingStep
    {
        Task ExecuteAsync(RecordContext context, CancellationToken ct);
    }
}
