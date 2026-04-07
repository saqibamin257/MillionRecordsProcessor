//using Hangfire;
//using Modules.Processing.Application;
//using Modules.Records.Infrastructure;
//using Processor.Worker;
//using Processor.Worker.Jobs;

//var builder = Host.CreateApplicationBuilder(args);
//builder.Services.AddHostedService<Worker>();

//builder.Services.AddHttpClient("external", client =>
//{
//    client.BaseAddress = new Uri("https://localhost:5227/");
//});


//builder.Services.AddHangfire(config =>
//    config.UseSqlServerStorage(
//        builder.Configuration.GetConnectionString("Default")));

//builder.Services.AddHangfireServer();

//builder.Services.AddScoped<ProcessingJobs>();


//builder.Services.AddProcessingModule();
//builder.Services.AddRecordsModule(
//builder.Configuration.GetConnectionString("Default"));


////var host = builder.Build();
////host.Run();


using Hangfire;
using Hangfire.SqlServer;
using Modules.Processing.Application;
using Modules.Processing.Application.Jobs;
using Modules.Processing.Application.Services;
using Modules.Records.Infrastructure;
//using Processor.Worker.Jobs;

var builder = Host.CreateApplicationBuilder(args);

// 🔹 Get connection string
var connectionString = builder.Configuration.GetConnectionString("Default");

// 🔹 Register Hangfire
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(connectionString));

builder.Services.AddHangfireServer();

// 🔹 Register modules
builder.Services.AddProcessingModule();
builder.Services.AddRecordsModule(connectionString);

// 🔹 Register jobs
builder.Services.AddScoped<ProcessingJobs>();

var app = builder.Build();



// 🔥 REGISTER RECURRING JOBS HERE (IMPORTANT)
using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider
        .GetRequiredService<IRecurringJobManager>();

    recurringJobManager.AddOrUpdate<ProcessingJobs>(
        "daily-processing-job",
        job => job.StartProcessing(),
        "0 8 * * *"
    );

    recurringJobManager.AddOrUpdate<ProcessingJobs>(
        "retry-failed-job",
        job => job.RetryFailed(),
        "*/10 * * * *"
    );
}

app.Run();