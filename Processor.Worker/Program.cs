using Hangfire;
using Hangfire.SqlServer;
using Modules.Processing.Application;
using Modules.Processing.Application.Jobs;
using Modules.Processing.Application.Services;
using Modules.Records.Infrastructure;
using Serilog;


//using Processor.Worker.Jobs;
var builder = Host.CreateApplicationBuilder(args);

// 🔹 Get connection string
var connectionString = builder.Configuration.GetConnectionString("Default");

// 🔹 Register Hangfire
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(connectionString));

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = Environment.ProcessorCount * 2;
    options.Queues = new[] { "processing", "retry", "default" };
});

// 🔹 Register modules
builder.Services.AddProcessingModule();
builder.Services.AddRecordsModule(connectionString);

// 🔹 Register jobs
builder.Services.AddScoped<ProcessingJobs>();


builder.Services.AddHttpClient("external", client =>
{
    client.BaseAddress = new Uri("https://localhost:7261/");
});



// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // information globally
    .WriteTo.Console()    // optional (can keep info)
    .WriteTo.File(
        "logs/error-log-.txt",
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error, //  only errors in file
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}"
    )
    .CreateLogger();

// Plug Serilog into logging pipeline
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();



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