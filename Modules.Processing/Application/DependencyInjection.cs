using Microsoft.Extensions.DependencyInjection;
using Modules.Processing.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Processing.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddProcessingModule(this IServiceCollection services)
        {
            services.AddScoped<ProcessingService>();

            return services;
        }
    }
}
