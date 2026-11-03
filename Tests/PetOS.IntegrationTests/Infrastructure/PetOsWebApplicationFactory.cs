using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using PetOS.Data;
using PetOS.Models;
using PetOS.Services;

namespace PetOS.IntegrationTests.Infrastructure;

public class PetOsWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private const string TestJwtKey =
        "ChaveDeTestePetOSNaoUtilizarEmProducao-2026!";

    private const string TestJwtIssuer = "PetOS";
    private const string TestJwtAudience = "PetOS-API";

    public HttpClient CreateAuthenticatedClient()
    {
        var client = CreateClient();

        var jwtService = Services
            .GetRequiredService<JwtTokenService>();

        var testUser = new AppUser
        {
            Id = 1,
            Name = "Tutor de Teste",
            Email = "tutor@petos.test"
        };

        var token = jwtService.CreateToken(testUser);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token.AccessToken
            );

        return client;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = TestJwtKey,
                    ["Jwt:Issuer"] = TestJwtIssuer,
                    ["Jwt:Audience"] = TestJwtAudience
                });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(
                    "PetOsIntegrationTests"
                );
            });
            
            services.PostConfigure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = TestJwtIssuer,

                            ValidateAudience = true,
                            ValidAudience = TestJwtAudience,

                            ValidateLifetime = true,

                            ValidateIssuerSigningKey = true,

                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(TestJwtKey)
                                ),

                            ClockSkew = TimeSpan.FromMinutes(1)
                        };
                });
        });
    }
}