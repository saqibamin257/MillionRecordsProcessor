using System;
using System.Collections.Generic;
using System.Text;
using Modules.Records.Domain;

namespace Modules.Records.Application.Interfaces
{
    public interface IRecordRepository
    {
        Task AddRangeAsync(List<Record> records);
        Task<int> GetTotalCountAsync();
        Task<int> GetProcessedCountAsync();
        Task AddLogsAsync(List<ProcessingLog> logs);

        Task<List<ProcessingLog>> GetFailedLogsAsync();
        Task<Record?> GetByIdAsync(Guid id);
        Task UpdateLogsAsync(List<ProcessingLog> logs);
    }
}
