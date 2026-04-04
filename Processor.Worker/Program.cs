using Hangfire;
using Modules.Processing.Application;
using Modules.Records.Infrastructure;
using Processor.Worker;
using Processor.Worker.Jobs;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddHttpClient("external", client =>
{
    client.BaseAddress = new Uri("https://localhost:5227/");
});


builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(
        builder.Configuration.GetConnectionString("Default")));

builder.Services.AddHangfireServer();

builder.Services.AddScoped<ProcessingJobs>();


builder.Services.AddProcessingModule();
builder.Services.AddRecordsModule(
builder.Configuration.GetConnectionString("Default"));


//var host = builder.Build();
//host.Run();
