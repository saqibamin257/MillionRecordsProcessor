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

            services.AddHttpClient("external", client =>
            {
                client.BaseAddress = new Uri("http://localhost:5227/");
            });

            services.AddScoped<ProcessingService>();

            return services;
        }
    }
}
