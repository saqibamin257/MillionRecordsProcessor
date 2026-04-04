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
    }
}
