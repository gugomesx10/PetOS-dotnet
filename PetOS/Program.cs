using Microsoft.EntityFrameworkCore;
using PetOS.Data;
using PetOS.Repositories;
using PetOS.Services;
using PetOS.Repositories.Interfaces;
using PetOS.Services.Interfaces;
using System.Reflection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using PetOS.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

app.Run();