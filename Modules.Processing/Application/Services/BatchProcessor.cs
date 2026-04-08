using Modules.Processing.Application.Context;
using Modules.Processing.Application.Pipeline;
using Modules.Records.Application.Interfaces;
using Modules.Records.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Processing.Application.Services
{
    public class BatchProcessor
    {
        private readonly ProcessingPipeline _pipeline;
        private readonly IRecordRepository _repository;

        public BatchProcessor(ProcessingPipeline pipeline, IRecordRepository repository)
        {
            _pipeline = pipeline;
            _repository = repository;
        }

        public async Task ProcessAsync(List<Record> records)
        {
            var contexts = records.Select(r => new RecordContext { Record = r }).ToList();

            var throttler = new SemaphoreSlim(10); // control concurrency

            var tasks = contexts.Select(async ctx =>
            {
                await throttler.WaitAsync();
                try
                {
                    await _pipeline.ExecuteAsync(ctx, CancellationToken.None);
                }
                finally
                {
                    throttler.Release();
                }
            });

            await Task.WhenAll(tasks);

            await _repository.AddRangeAsync(records);

            var failedLogs = contexts
                .Where(c => c.HasFailed)
                .Select(c => new ProcessingLog(c.Record.Id, c.Error!))
                .ToList();

            if (failedLogs.Any())
                await _repository.AddLogsAsync(failedLogs);
        }
    }
}
