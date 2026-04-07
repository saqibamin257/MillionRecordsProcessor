using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Records.Infrastructure
{
    public class RecordsDbContextFactory
    : IDesignTimeDbContextFactory<RecordsDbContext>
    {
        public RecordsDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RecordsDbContext>();

            optionsBuilder.UseSqlServer(
                "Server=localhost\\SQLEXPRESS;Database=MillionRecordsDb;Trusted_Connection=True;TrustServerCertificate=True;");

            return new RecordsDbContext(optionsBuilder.Options);
        }
    }
}
