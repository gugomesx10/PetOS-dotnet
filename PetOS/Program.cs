using Microsoft.EntityFrameworkCore;
using PetOS.Data;
using PetOS.Repositories;
using PetOS.Services;
using PetOS.Repositories.Interfaces;
using PetOS.Services.Interfaces;
using System.Reflection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using PetOS.HealthChecks;
using Serilog;
using Serilog.Events;
using PetOS.Middleware;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

const string logTemplate =
    "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] " +
    "[CorrelationId:{CorrelationId}] " +
    "{Message:lj}{NewLine}{Exception}";

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console(
            outputTemplate: logTemplate
        )
        .WriteTo.File(
            "logs/petos-.log",
            rollingInterval: RollingInterval.Day,
            outputTemplate: logTemplate
        );
});

builder.Services.AddDbContext<AppDbContext>(options => {
    options.UseOracle(builder.Configuration.GetConnectionString("Oracle"));
});

// healtcheks
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>(
        name: "oracle",
        tags: new[] { "ready" }
    );

// Services
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IRoutineService, RoutineService>();
builder.Services.AddScoped<IVaccineService, VaccineService>();
builder.Services.AddScoped<IPetService, PetService>();
// Repositories
builder.Services.AddScoped<IPetRepository, PetRepository>();
builder.Services.AddScoped<IVaccineRepository, VaccineRepository>();
builder.Services.AddScoped<IRoutineRepository, RoutineRepository>();
builder.Services.AddScoped<IAlertRepository, AlertRepository>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { 
    c.EnableAnnotations();
    
    var  xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource =>
    {
        resource.AddService("PetOS");
    })
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddSource("PetOS")
            .AddConsoleExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddPrometheusExporter();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, elapsed, exception) =>
    {
        if (exception != null || httpContext.Response.StatusCode >= 500)
        {
            return Serilog.Events.LogEventLevel.Error;
        }

        if (httpContext.Response.StatusCode >= 400)
        {
            return Serilog.Events.LogEventLevel.Warning;
        }

        return Serilog.Events.LogEventLevel.Information;
    };
});

app.UseAuthorization();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = HealthCheckResponseWriter.WriteResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = HealthCheckResponseWriter.WriteResponse
});

app.MapControllers();

app.MapPrometheusScrapingEndpoint();

app.Run();

public partial class Program
{
}