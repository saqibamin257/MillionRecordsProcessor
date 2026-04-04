using Microsoft.EntityFrameworkCore;
using Modules.Records.Application.Interfaces;
using Modules.Records.Domain;
using System;
using System.Collections.Generic;
using System.Text;


namespace Modules.Records.Infrastructure
{
    public class RecordRepository : IRecordRepository
    {
        private readonly RecordsDbContext _db;
        public RecordRepository(RecordsDbContext db) 
        {
            this._db = db;
        }
        public async Task AddRangeAsync(List<Record> records)
        {
            _db.Records.AddRangeAsync(records);
            _db.SaveChangesAsync();            
        }

        public  Task<int> GetProcessedCountAsync()
        {
           return _db.Records.CountAsync(x => x.Status == "Processed");
        }

        public Task<int> GetTotalCountAsync()
        {
            return _db.Records.CountAsync();
        }
    }
}
