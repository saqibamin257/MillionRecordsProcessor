using Microsoft.Extensions.DependencyInjection;
using Modules.Processing.Application.Pipeline;
using Modules.Processing.Application.Services;
using Modules.Processing.Application.Steps;
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

           
            services.AddScoped<BatchProcessor>();
            services.AddScoped<ProcessingPipeline>();
            services.AddScoped<IProcessingStep, ProcessingStep>();
            services.AddScoped<ProcessingService>();

            return services;
        }
    }
}
