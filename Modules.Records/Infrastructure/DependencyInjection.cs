using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Records.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Records.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRecordsModule(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddDbContext<RecordsDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IRecordRepository, RecordRepository>();

            return services;
        }
    }
}
