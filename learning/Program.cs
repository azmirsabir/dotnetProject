using System.Text.Json.Serialization;
using learning.Exceptions;
using learning.Middlewares;
using learning.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCorsPolicy(builder.Configuration);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddOpenApi();
builder.Services.AddControllers(options => { options.Filters.Add(new GlobalExceptionHandler()); })
.AddJsonOptions(opt => { opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());});

builder.Services.AddGlobalFixedWindowRateLimiting();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerAuth();

var app = builder.Build();

app.UseMiddleware<LoggingMiddleware>();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();


if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
    app.UseCors("AllowAll");
    await app.UseDatabaseSeeder();
}

if (app.Environment.IsProduction())
{
    app.UseHsts();
    app.UseHttpsRedirection();
    app.UseCors("AllowTrusted");
}
app.MapControllers();
app.MapFallback(() => Results.NotFound("Endpoint not found"));
app.Run();