var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<MemoryMetricsStore>();
builder.Services.AddHostedService<MemoryMetricsCollector>();
builder.Services.AddSingleton<CPUMetricsStore>();
builder.Services.AddHostedService<CPUMetricsCollector>();
builder.Logging.AddFilter("MemoryMetricsCollector", LogLevel.None);
builder.Logging.AddFilter("CpuMetricsCollector", LogLevel.None);

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();
app.MapControllers();
app.Run();
