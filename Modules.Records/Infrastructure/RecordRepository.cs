using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Modules.Records.Application.Interfaces;
using Modules.Records.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.ConstrainedExecution;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Modules.Records.Infrastructure
{
    public class RecordRepository : IRecordRepository
    {
        private readonly RecordsDbContext _db;
        public RecordRepository(RecordsDbContext db) 
        {
            this._db = db;
        }

        //public async Task AddRangeAsync(List<Record> records)
        //{
        //    var table = new DataTable();

        //    table.Columns.Add("Id", typeof(Guid));
        //    table.Columns.Add("Name", typeof(string));
        //    table.Columns.Add("Email", typeof(string));
        //    table.Columns.Add("Status", typeof(string));
        //    table.Columns.Add("ProcessedAt", typeof(DateTime));

        //    foreach (var r in records)
        //    {
        //        table.Rows.Add(
        //            r.Id,
        //            r.Name,
        //            r.Email,
        //            r.Status,
        //            r.ProcessedAt ?? (object)DBNull.Value
        //        );
        //    }

        //    var connection = (SqlConnection)_db.Database.GetDbConnection();

        //    if (connection.State != ConnectionState.Open)
        //        await connection.OpenAsync();

        //    using var transaction = await _db.Database.BeginTransactionAsync();
        //    var dbTransaction = (SqlTransaction)transaction.GetDbTransaction();

        //    using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, dbTransaction)
        //    {
        //        DestinationTableName = "Records_Staging"
        //    };

        //    try
        //    {
        //        //Bulk insert → staging
        //        await bulkCopy.WriteToServerAsync(table);

        //        //Insert only new records
        //        await _db.Database.ExecuteSqlRawAsync(@"
        //                                    INSERT INTO Records (Id, Name, Email, Status, ProcessedAt)
        //                                    SELECT s.Id, s.Name, s.Email, s.Status, s.ProcessedAt
        //                                    FROM Records_Staging s
        //                                    LEFT JOIN Records r ON r.Email = s.Email
        //                                    WHERE r.Email IS NULL
        //                                ");
        //        //Clear staging
        //        await _db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Records_Staging");
        //        await transaction.CommitAsync();
        //    }
        //    catch (Exception ex) 
        //    {
        //        await transaction.RollbackAsync();
        //        throw;
        //    }            
        //}


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

            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var dbTransaction = (SqlTransaction)transaction.GetDbTransaction();

                using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, dbTransaction)
                {
                    DestinationTableName = "Records_Staging"
                };

                // 1. Bulk insert → staging
                await bulkCopy.WriteToServerAsync(table);

                // 2. Insert only new records
                await _db.Database.ExecuteSqlRawAsync(@"
                                                    INSERT INTO Records (Id, Name, Email, Status, ProcessedAt)
                                                    SELECT s.Id, s.Name, s.Email, s.Status, s.ProcessedAt
                                                    FROM Records_Staging s
                                                    LEFT JOIN Records r ON r.Email = s.Email
                                                    WHERE r.Email IS NULL
                                                ");

                // 3. Clear staging
                await _db.Database.ExecuteSqlRawAsync("DELETE FROM Records_Staging");

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                //_logger.LogError(ex, "Error while bulk inserting records batch. Count: {Count}", records.Count);

                //throw;
            }
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
