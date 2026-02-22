using Microsoft.EntityFrameworkCore;
using MinhaApi.Data;
using MinhaApi.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Registro do Redis
var redisConfig = builder.Configuration.GetSection("Redis:Configuration").Value;
var redisPassword = builder.Configuration.GetSection("Redis:Password").Value;
var abortOnConnectFail = builder.Configuration.GetValue<bool>("Redis:AbortOnConnectFail");

var configOptions = ConfigurationOptions.Parse(redisConfig!);
if (!string.IsNullOrEmpty(redisPassword))
{
    configOptions.Password = redisPassword;
}
configOptions.AbortOnConnectFail = abortOnConnectFail;
configOptions.ConnectTimeout = builder.Configuration.GetValue<int>("Redis:ConnectTimeout");
configOptions.ConnectRetry = builder.Configuration.GetValue<int>("Redis:ConnectRetry");

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configOptions));
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<ILoteService, LoteService>();

// Registro do Producer e Worker para filas Redis Streams
builder.Services.AddScoped<ILoteQueueProducer, LoteQueueProducer>();
builder.Services.AddHostedService<LoteQueueWorker>();

// Registro do DbContext com Npgsql

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var cs = "Host=localhost;Port=5433;Database=minhaapi_db;Username=postgres;Password=postgres";
    options
        .UseNpgsql(cs)
        .UseSnakeCaseNamingConvention();
});


// Recomendação do Npgsql para compatibilidade de timestamp (se aplicável)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

// Mapear controllers
app.MapControllers();

app.Run();

public partial class Program { }
