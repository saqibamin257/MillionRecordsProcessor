using Hangfire;
using Modules.Records.Application.Interfaces;
using Modules.Records.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddRecordsModule(
    builder.Configuration.GetConnectionString("Default"));



builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(
        builder.Configuration.GetConnectionString("Default")));


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});


var app = builder.Build();

app.UseCors("AllowAll");

app.UseHangfireDashboard();

app.MapGet("/stats", async (IRecordRepository repo) =>
{
    var total = await repo.GetTotalCountAsync();
    var processed = await repo.GetProcessedCountAsync();
    var failed = (await repo.GetFailedLogsAsync()).Count;

    return new
    {
        total,
        processed,
        failed
    };
});




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
