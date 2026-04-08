using Microsoft.Identity.Client;
using Modules.Processing.Application.Models;
using Modules.Records.Application.Interfaces;
using Modules.Records.Domain;
using Modules.Records.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;


namespace Modules.Processing.Application.Services
{
    public class ProcessingService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly BatchProcessor _batchProcessor;
        private readonly IRecordRepository _recordRepository;

        public ProcessingService(IHttpClientFactory httpClientFactory, IRecordRepository recordRepository, BatchProcessor batchProcessor)
        {
            this._httpClientFactory = httpClientFactory;
            this._recordRepository = recordRepository;
            this._batchProcessor = batchProcessor;
        }


        public async Task StartAsync()
        {
            try 
            {
                var client = _httpClientFactory.CreateClient("external");

                int page = 1;
                int pageSize = 500;

                while (true)
                {
                    var data = await client.GetFromJsonAsync<List<ExternalRecordDto>>(
                        $"api/data?page={page}&pageSize={pageSize}");

                    if (data == null || data.Count == 0)
                        break;

                    var records = data.Select(MapToDomain).ToList();

                    await _batchProcessor.ProcessAsync(records);

                    page++;
                }
            }
            catch (Exception ex) 
            {
            }            
        }

        private Record MapToDomain(ExternalRecordDto dto)
            => new Record(dto.Name, dto.Email);

        public async Task RetryFailedAsync()
        {
            try 
            {
                var failedLogs = await _recordRepository.GetFailedLogsAsync();

                var throttler = new SemaphoreSlim(5);

                var tasks = failedLogs.Select(async log =>
                {
                    await throttler.WaitAsync();
                    try
                    {
                        var record = await _recordRepository.GetByIdAsync(log.RecordId);

                        if (record == null) return;

                        await Task.Delay(10);

                        record.MarkProcessed();
                        log.MarkResolved();
                    }
                    catch
                    {
                        log.IncrementRetry();
                    }
                    finally
                    {
                        throttler.Release();
                    }
                });

                await Task.WhenAll(tasks);

                await _recordRepository.UpdateLogsAsync(failedLogs);
            }
            catch (Exception ex) { }            
        }
    }







    //public class ProcessingService
    //{
    //    private readonly IHttpClientFactory _httpClientFactory;
    //    private readonly IRecordRepository _recordRepository;

    //    public ProcessingService(IHttpClientFactory httpClientFactory, IRecordRepository recordRepository) 
    //    {
    //        this._httpClientFactory = httpClientFactory;
    //        this._recordRepository = recordRepository;
    //    }

    //public async Task StartAsync()
    //{
    //    var client = _httpClientFactory.CreateClient("external");

    //    int page = 1;
    //    int pageSize = 500;

    //    while (true)
    //    {
    //        var data = await client.GetFromJsonAsync<List<ExternalRecordDto>>(
    //            $"api/data?page={page}&pageSize={pageSize}");

    //        if (data == null || data.Count == 0)
    //            break;

    //        var records = data
    //            .Select(x => new Record(x.Name, x.Email))
    //            .ToList();                

    //        await ProcessBatch(records);
    //        page++;
    //    }
    //}


    //private async Task ProcessBatch(List<Record> records)
    //{
    //    var failedLogs = new List<ProcessingLog>();

    //    await Parallel.ForEachAsync(records, async (record, ct) =>
    //    {
    //        try
    //        {
    //            await Task.Delay(10, ct);

    //            if (Random.Shared.Next(1, 10) == 5)
    //                throw new Exception("Random failure");

    //            record.MarkProcessed();
    //        }
    //        catch (Exception ex)
    //        {
    //            record.MarkFailed();

    //            lock (failedLogs) // thread safety
    //            {
    //                failedLogs.Add(new ProcessingLog(record.Id, ex.Message));
    //            }
    //        }
    //    });

    //    await _recordRepository.AddRangeAsync(records);

    //    if (failedLogs.Any())
    //        await _recordRepository.AddLogsAsync(failedLogs);
    //}


    //public async Task RetryFailedAsync()
    //{
    //    var failedLogs = await _recordRepository.GetFailedLogsAsync();

    //    var logsToUpdate = new List<ProcessingLog>();

    //    foreach (var log in failedLogs)
    //    {
    //        try
    //        {
    //            var record = await _recordRepository.GetByIdAsync(log.RecordId);

    //            if (record == null)
    //                continue;

    //            // Reprocess
    //            await Task.Delay(10);

    //            record.MarkProcessed();

    //            log.MarkResolved();
    //        }
    //        catch
    //        {
    //            log.IncrementRetry();
    //        }

    //        logsToUpdate.Add(log);
    //    }

    //    await _recordRepository.UpdateLogsAsync(logsToUpdate);
    //}
    // }
}
