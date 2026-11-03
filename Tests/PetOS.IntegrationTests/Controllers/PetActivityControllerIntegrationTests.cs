using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PetOS.Data;
using PetOS.Dto.Common;
using PetOS.Dto.Pet;
using PetOS.Dto.PetActivity;
using PetOS.IntegrationTests.Infrastructure;
using PetOS.Models;
using PetOS.Repositories.Interfaces;

namespace PetOS.IntegrationTests.Controllers;

[Collection("PetOS Integration Collection")]
public sealed class PetActivityControllerIntegrationTests : IDisposable
{
    private const string ValidId = "507f1f77bcf86cd799439011";

    private readonly PetActivityTestFactory _factory = new();
    private readonly HttpClient _client;

    public PetActivityControllerIntegrationTests()
    {
        _client = _factory.CreateAuthenticatedClient();

        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    private static async Task<JsonElement> ReadJsonAsync(
        HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(content);

        return document.RootElement.Clone();
    }

    private async Task<long> CreatePetAsync(string name = "Thor")
    {
        var dto = new PetCreateDto
        {
            Name = name,
            Species = "Cachorro",
            Breed = "Vira-lata",
            BirthDate = new DateTime(2022, 5, 10),
            Gender = "Macho",
            weight = 12
        };

        using var response = await _client.PostAsJsonAsync(
            "/api/Pet",
            dto
        );

        response.EnsureSuccessStatusCode();

        var json = await ReadJsonAsync(response);

        return json
            .GetProperty("data")
            .GetProperty("id")
            .GetInt64();
    }

    private async Task<string> CreateActivityAsync(
        long petId,
        string type = "Passeio",
        string description = "Caminhada no parque")
    {
        var dto = new PetActivityCreateDto
        {
            PetId = petId,
            Type = type,
            Description = description,
            Metadata = new Dictionary<string, string>
            {
                ["durationMinutes"] = "30"
            }
        };

        using var response = await _client.PostAsJsonAsync(
            "/api/PetActivity",
            dto
        );

        response.EnsureSuccessStatusCode();

        var json = await ReadJsonAsync(response);

        return json
            .GetProperty("data")
            .GetProperty("id")
            .GetString()!;
    }

    // =====================================================
    // POST - Criação
    // =====================================================

    [Fact]
    public async Task Create_ValidActivity_ReturnsCreatedAndHateoas()
    {
        // Arrange
        var petId = await CreatePetAsync();

        var dto = new PetActivityCreateDto
        {
            PetId = petId,
            Type = "Passeio",
            Description = "Caminhada de 30 minutos",
            Metadata = new Dictionary<string, string>
            {
                ["location"] = "Parque Ibirapuera"
            }
        };

        // Act
        using var response = await _client.PostAsJsonAsync(
            "/api/PetActivity",
            dto
        );

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var json = await ReadJsonAsync(response);

        Assert.Equal(
            "Atividade criada com sucesso",
            json.GetProperty("message").GetString()
        );

        var data = json.GetProperty("data");

        Assert.Equal(petId, data.GetProperty("petId").GetInt64());
        Assert.Equal("Passeio", data.GetProperty("type").GetString());

        var id = data.GetProperty("id").GetString();

        Assert.False(string.IsNullOrWhiteSpace(id));

        Assert.Contains(
            json.GetProperty("links").EnumerateArray(),
            link =>
                link.GetProperty("rel").GetString() == "self" &&
                link.GetProperty("href").GetString() ==
                $"/api/PetActivity/{id}"
        );
    }

    [Fact]
    public async Task Create_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        using var anonymousClient = _factory.CreateClient();

        var dto = new PetActivityCreateDto
        {
            PetId = 1,
            Type = "Passeio",
            Description = "Atividade não autorizada"
        };

        // Act
        using var response = await anonymousClient.PostAsJsonAsync(
            "/api/PetActivity",
            dto
        );

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode
        );
    }

    [Fact]
    public async Task Create_PetDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var dto = new PetActivityCreateDto
        {
            PetId = 999,
            Type = "Consulta",
            Description = "Consulta veterinária"
        };

        // Act
        using var response = await _client.PostAsJsonAsync(
            "/api/PetActivity",
            dto
        );

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    // =====================================================
    // GET - Consultas
    // =====================================================

    [Fact]
    public async Task GetById_ExistingActivity_ReturnsOk()
    {
        // Arrange
        var petId = await CreatePetAsync();
        var activityId = await CreateActivityAsync(petId);

        // Act
        using var response = await _client.GetAsync(
            $"/api/PetActivity/{activityId}"
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await ReadJsonAsync(response);

        Assert.Equal(
            activityId,
            json.GetProperty("data")
                .GetProperty("id")
                .GetString()
        );

        Assert.Contains(
            json.GetProperty("links").EnumerateArray(),
            link => link.GetProperty("rel").GetString() == "self"
        );
    }

    [Fact]
    public async Task GetById_InvalidId_ReturnsBadRequest()
    {
        // Act
        using var response = await _client.GetAsync(
            "/api/PetActivity/id-invalido"
        );

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode
        );

        var json = await ReadJsonAsync(response);

        Assert.Equal(
            400,
            json.GetProperty("status").GetInt32()
        );
    }

    [Fact]
    public async Task GetById_ActivityDoesNotExist_ReturnsNotFound()
    {
        // Act
        using var response = await _client.GetAsync(
            $"/api/PetActivity/{ValidId}"
        );

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    [Fact]
    public async Task GetAll_NoActivities_ReturnsNoContent()
    {
        // Act
        using var response = await _client.GetAsync(
            "/api/PetActivity"
        );

        // Assert
        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode
        );
    }

    [Fact]
    public async Task GetAll_PaginationAndFilters_ReturnsMetadataAndLinks()
    {
        // Arrange
        var petId = await CreatePetAsync();

        await CreateActivityAsync(petId, "Passeio", "Passeio 1");
        await CreateActivityAsync(petId, "Passeio", "Passeio 2");
        await CreateActivityAsync(petId, "Consulta", "Consulta 1");

        var url =
            $"/api/PetActivity?petId={petId}" +
            "&type=Passeio" +
            "&page=1" +
            "&pageSize=1" +
            "&sortBy=createdAtUtc" +
            "&sortDirection=desc";

        // Act
        using var response = await _client.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await ReadJsonAsync(response);

        var pagination = json.GetProperty("pagination");

        Assert.Equal(
            2,
            pagination.GetProperty("totalItems").GetInt32()
        );

        Assert.Equal(
            1,
            pagination.GetProperty("page").GetInt32()
        );

        Assert.Equal(
            1,
            pagination.GetProperty("pageSize").GetInt32()
        );

        Assert.Equal(
            2,
            pagination.GetProperty("totalPages").GetInt32()
        );

        Assert.True(
            pagination.GetProperty("hasNext").GetBoolean()
        );

        Assert.Single(
            json.GetProperty("data").EnumerateArray()
        );

        Assert.Equal(
            "Passeio",
            json.GetProperty("data")[0]
                .GetProperty("type")
                .GetString()
        );

        Assert.Contains(
            json.GetProperty("links").EnumerateArray(),
            link => link.GetProperty("rel").GetString() == "next"
        );
    }

    [Fact]
    public async Task GetAll_InvalidPage_ReturnsBadRequest()
    {
        // Act
        using var response = await _client.GetAsync(
            "/api/PetActivity?page=0&pageSize=10"
        );

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode
        );
    }

    [Fact]
    public async Task GetByPetId_ReturnsOnlyPetActivities()
    {
        // Arrange
        var firstPetId = await CreatePetAsync("Thor");
        var secondPetId = await CreatePetAsync("Luna");

        await CreateActivityAsync(firstPetId, "Passeio");
        await CreateActivityAsync(secondPetId, "Consulta");

        // Act
        using var response = await _client.GetAsync(
            $"/api/PetActivity/pet/{firstPetId}"
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await ReadJsonAsync(response);

        Assert.Single(
            json.GetProperty("data").EnumerateArray()
        );

        Assert.Equal(
            firstPetId,
            json.GetProperty("data")[0]
                .GetProperty("petId")
                .GetInt64()
        );
    }

    [Fact]
    public async Task GetByPetId_PetDoesNotExist_ReturnsNotFound()
    {
        // Act
        using var response = await _client.GetAsync(
            "/api/PetActivity/pet/999"
        );

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    // =====================================================
    // PUT - Atualização
    // =====================================================

    [Fact]
    public async Task Update_ExistingActivity_ReturnsOk()
    {
        // Arrange
        var petId = await CreatePetAsync();
        var activityId = await CreateActivityAsync(petId);

        var dto = new PetActivityCreateDto
        {
            PetId = petId,
            Type = "Passeio",
            Description = "Caminhada atualizada",
            Metadata = new Dictionary<string, string>
            {
                ["durationMinutes"] = "60"
            }
        };

        // Act
        using var response = await _client.PutAsJsonAsync(
            $"/api/PetActivity/{activityId}",
            dto
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await ReadJsonAsync(response);

        Assert.Equal(
            "Atividade atualizada com sucesso",
            json.GetProperty("message").GetString()
        );

        // Verifica se os dados foram alterados
        using var getResponse = await _client.GetAsync(
            $"/api/PetActivity/{activityId}"
        );

        var updatedJson = await ReadJsonAsync(getResponse);

        Assert.Equal(
            "Caminhada atualizada",
            updatedJson.GetProperty("data")
                .GetProperty("description")
                .GetString()
        );

        Assert.Equal(
            "60",
            updatedJson.GetProperty("data")
                .GetProperty("metadata")
                .GetProperty("durationMinutes")
                .GetString()
        );
    }

    [Fact]
    public async Task Update_ActivityDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var dto = new PetActivityCreateDto
        {
            PetId = 1,
            Type = "Passeio",
            Description = "Atualização"
        };

        // Act
        using var response = await _client.PutAsJsonAsync(
            $"/api/PetActivity/{ValidId}",
            dto
        );

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    // =====================================================
    // DELETE - Exclusão
    // =====================================================

    [Fact]
    public async Task Delete_ExistingActivity_ReturnsOk()
    {
        // Arrange
        var petId = await CreatePetAsync();
        var activityId = await CreateActivityAsync(petId);

        // Act
        using var response = await _client.DeleteAsync(
            $"/api/PetActivity/{activityId}"
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await ReadJsonAsync(response);

        Assert.Equal(
            "Atividade excluída com sucesso",
            json.GetProperty("message").GetString()
        );

        // Confirma que não existe mais
        using var getResponse = await _client.GetAsync(
            $"/api/PetActivity/{activityId}"
        );

        Assert.Equal(
            HttpStatusCode.NotFound,
            getResponse.StatusCode
        );
    }

    [Fact]
    public async Task Delete_ActivityDoesNotExist_ReturnsNotFound()
    {
        // Act
        using var response = await _client.DeleteAsync(
            $"/api/PetActivity/{ValidId}"
        );

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}

/// <summary>
/// Factory de testes HTTP que mantém o Oracle em memória
/// e substitui somente o repositório MongoDB.
/// </summary>
internal sealed class PetActivityTestFactory
    : PetOsWebApplicationFactory
{
    public FakePetActivityRepository Repository { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IPetActivityRepository>();

            services.AddSingleton<IPetActivityRepository>(
                Repository
            );
        });
    }
}

/// <summary>
/// Repositório simulado exclusivo dos testes HTTP.
/// Não acessa o MongoDB Atlas.
/// </summary>
internal sealed class FakePetActivityRepository
    : IPetActivityRepository
{
    private readonly Dictionary<string, PetActivity> _activities = new();

    public Task<PagedResult<PetActivity>> GetPagedAsync(
        PetActivityQuery query)
    {
        IEnumerable<PetActivity> filtered = _activities.Values;

        if (query.PetId.HasValue)
        {
            filtered = filtered.Where(activity =>
                activity.PetId == query.PetId.Value
            );
        }

        if (!string.IsNullOrWhiteSpace(query.Type))
        {
            filtered = filtered.Where(activity =>
                string.Equals(
                    activity.Type,
                    query.Type.Trim(),
                    StringComparison.Ordinal
                )
            );
        }

        Func<PetActivity, IComparable> sortKey =
            (query.SortBy ?? string.Empty)
                .Trim()
                .ToLowerInvariant() switch
            {
                "id" => activity => activity.Id,
                "petid" => activity => activity.PetId,
                "type" => activity => activity.Type,
                "createdatutc" => activity => activity.CreatedAtUtc,

                _ => throw new ArgumentException(
                    "Campo de ordenação inválido."
                )
            };

        var descending = string.Equals(
            query.SortDirection,
            "desc",
            StringComparison.OrdinalIgnoreCase
        );

        var sorted = descending
            ? filtered.OrderByDescending(sortKey)
            : filtered.OrderBy(sortKey);

        var results = sorted.ToList();
        var totalCount = results.Count;

        var items = results
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        var page = new PagedResult<PetActivity>(
            items,
            query.Page,
            query.PageSize,
            totalCount
        );

        return Task.FromResult(page);
    }

    public Task<PetActivity?> GetByIdAsync(string id)
    {
        _activities.TryGetValue(id, out var activity);

        return Task.FromResult(activity);
    }

    public Task InsertAsync(PetActivity activity)
    {
        _activities.Add(activity.Id, activity);

        return Task.CompletedTask;
    }

    public Task<bool> ReplaceAsync(PetActivity activity)
    {
        if (!_activities.ContainsKey(activity.Id))
        {
            return Task.FromResult(false);
        }

        _activities[activity.Id] = activity;

        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(string id)
    {
        var deleted = _activities.Remove(id);

        return Task.FromResult(deleted);
    }
}