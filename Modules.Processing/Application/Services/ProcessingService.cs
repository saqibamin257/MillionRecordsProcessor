using Microsoft.Identity.Client;
using Modules.Processing.Application.Models;
using Modules.Records.Application.Interfaces;
using Modules.Records.Domain;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;


namespace Modules.Processing.Application.Services
{
    public class ProcessingService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRecordRepository _recordRepository;

        public ProcessingService(IHttpClientFactory httpClientFactory, IRecordRepository recordRepository) 
        {
            this._httpClientFactory = httpClientFactory;
            this._recordRepository = recordRepository;
        }

        public async Task StartAsync()
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

                var records = data
                    .Select(x => new Record(x.Name, x.Email))
                    .ToList();                
                
                await ProcessBatch(records);
                page++;
            }
        }

        //public async Task ProcessBatch(List<Record> records)
        //{
        //    foreach (var record in records)
        //    {
        //        try
        //        {
        //             // Simulate random failure
        //             if (Random.Shared.Next(1, 10) == 5)
        //                throw new Exception("Random failure");
                    
        //             // Simulate heavy processing
        //            await Task.Delay(10);

        //            record.MarkProcessed();
        //        }
        //        catch
        //        {
        //            record.MarkFailed();
        //        }
        //    }

        //    await _recordRepository.AddRangeAsync(records);
        //}

        private async Task ProcessBatch(List<Record> records)
        {
            await Parallel.ForEachAsync(records, async (record, ct) =>
            {
                try
                {
                    // Simulate work
                    await Task.Delay(10, ct);

                    if (Random.Shared.Next(1, 10) == 5)
                        throw new Exception("Random failure");

                    record.MarkProcessed();
                }
                catch
                {
                    record.MarkFailed();
                }
            });

            await _recordRepository.AddRangeAsync(records);
        }
    }
}
