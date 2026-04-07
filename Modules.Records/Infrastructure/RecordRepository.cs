using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Modules.Records.Application.Interfaces;
using Modules.Records.Domain;
using System;
using System.Collections.Generic;
using System.Data;
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
            var table = new DataTable();

            table.Columns.Add("Id", typeof(Guid));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("ProcessedAt", typeof(DateTime));

            foreach (var r in records)
            {
                table.Rows.Add(
                    r.Id,
                    r.Name,
                    r.Email,
                    r.Status,
                    r.ProcessedAt ?? (object)DBNull.Value
                );
            }

            var connection = (SqlConnection)_db.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            using var bulkCopy = new SqlBulkCopy(connection)
            {
                DestinationTableName = "Records"
            };

            await bulkCopy.WriteToServerAsync(table);
        }

        public  Task<int> GetProcessedCountAsync()
        {
           return _db.Records.CountAsync(x => x.Status == "Processed");
        }

        public Task<int> GetTotalCountAsync()
        {
            return _db.Records.CountAsync();
        }
        public async Task AddLogsAsync(List<ProcessingLog> logs)
        {
            await _db.ProcessingLogs.AddRangeAsync(logs);
            await _db.SaveChangesAsync();
        }

        public async Task<List<ProcessingLog>> GetFailedLogsAsync()
        {
            return await _db.ProcessingLogs
                .Where(x => x.Status == "Failed" && x.RetryCount < 3)
                .ToListAsync();
        }
        public async Task<Record?> GetByIdAsync(Guid id)
        {
            return await _db.Records.FindAsync(id);
        }

        public async Task UpdateLogsAsync(List<ProcessingLog> logs)
        {
            _db.ProcessingLogs.UpdateRange(logs);
            await _db.SaveChangesAsync();
        }
    }
}
