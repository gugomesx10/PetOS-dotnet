using Microsoft.EntityFrameworkCore;
using PetOS.Data;
using PetOS.Exceptions;
using PetOS.Repositories;
using PetOS.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("OracleDb")
                           ?? throw new InvalidOperationException("Connection string 'OracleDb' nao configurada.");
    options.UseOracle(connectionString);
});

builder.Services.AddScoped<IPetRepository, PetRepository>();
builder.Services.AddScoped<IVacinaRepository, VacinaRepository>();
builder.Services.AddScoped<IRotinaRepository, RotinaRepository>();
builder.Services.AddScoped<IAlertaRepository, AlertaRepository>();

builder.Services.AddScoped<IPetService, PetService>();
builder.Services.AddScoped<IVacinaService, VacinaService>();
builder.Services.AddScoped<IRotinaService, RotinaService>();
builder.Services.AddScoped<IAlertaService, AlertaService>();

var app = builder.Build();

app.UseMiddleware<ApiExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.MapControllers();

app.Run();
