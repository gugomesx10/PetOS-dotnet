using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PetOS.Data;
using PetOS.HealthChecks;
using PetOS.Middleware;
using PetOS.Models;
using PetOS.Repositories;
using PetOS.Repositories.Interfaces;
using PetOS.Services;
using PetOS.Services.Interfaces;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey) ||
    Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException(
        "A chave JWT não foi configurada. " +
        "Configure Jwt:Key com pelo menos 32 bytes."
    );
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "PetOS",

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "PetOS-API",

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

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

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseOracle(
        builder.Configuration.GetConnectionString("Oracle")
    );
});

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<AppDbContext>(
        name: "oracle",
        tags: new[] { "ready" }
    );

builder.Services.AddScoped<IPetService, PetService>();
builder.Services.AddScoped<IVaccineService, VaccineService>();
builder.Services.AddScoped<IRoutineService, RoutineService>();
builder.Services.AddScoped<IAlertService, AlertService>();

// Autenticação
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddSingleton<JwtTokenService>();

builder.Services.AddScoped<
    IPasswordHasher<AppUser>,
    PasswordHasher<AppUser>
>();

builder.Services.AddScoped<IPetRepository, PetRepository>();
builder.Services.AddScoped<IVaccineRepository, VaccineRepository>();
builder.Services.AddScoped<IRoutineRepository, RoutineRepository>();
builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{ 
    options.EnableAnnotations();

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PetOS | API de Gestão Veterinária",
        Version = "v1",

        Description = """
            API RESTful desenvolvida para o gerenciamento de
            cuidados com animais de estimação.

            O PetOS disponibiliza funcionalidades para:

            - Cadastro e gerenciamento de pets.
            - Controle do histórico de vacinação.
            - Registro de rotinas e cuidados.
            - Gerenciamento de alertas.
            - Cadastro e autenticação de usuários.
            - Autorização de operações utilizando JWT.

            Tecnologias principais:
            ASP.NET Core 8, Entity Framework Core e Oracle Database.

            Autenticação:
            As operações de consulta são públicas.
            Para cadastrar, atualizar ou excluir registros,
            é necessário utilizar um token JWT válido.

            Para obter um token, utilize POST /api/Auth/login.
            """,

        Contact = new OpenApiContact
        {
            Name = "Equipe PetOS",
            Url = new Uri(
                "https://github.com/gugomesx10/PetOS-dotnet"
            )
        }
    });

    options.TagActionsBy(api =>
    {
        var controller = api.ActionDescriptor.RouteValues["controller"];

        var category = controller switch
        {
            "Auth" => "01 - Autenticação",
            "Pet" => "02 - Pets",
            "Vaccine" => "03 - Vacinação",
            "Routine" => "04 - Rotinas e Cuidados",
            "Alert" => "05 - Alertas",
            _ => "06 - Outros"
        };

        return new[] { category };
    });

    var xmlFile =
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    var xmlPath = Path.Combine(
        AppContext.BaseDirectory,
        xmlFile
    );

    options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",

            Description =
                "Insira o token JWT obtido através do endpoint " +
                "POST /api/Auth/login."
        }
    );

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        }
    );
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "PetOS API - v1"
        );

        options.DocumentTitle =
            "PetOS | Documentação da API";
    });
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, elapsed, exception) =>
    {
        if (exception != null ||
            httpContext.Response.StatusCode >= 500)
        {
            return LogEventLevel.Error;
        }

        if (httpContext.Response.StatusCode >= 400)
        {
            return LogEventLevel.Warning;
        }

        return LogEventLevel.Information;
    };
});

app.UseAuthentication();
app.UseAuthorization();

// Verifica se a aplicação está em execução.
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,

    ResponseWriter =
        HealthCheckResponseWriter.WriteResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),

    ResponseWriter =
        HealthCheckResponseWriter.WriteResponse
});

app.MapControllers();
app.MapPrometheusScrapingEndpoint();
app.Run();

// Necessário para os testes de integração.
public partial class Program
{
}