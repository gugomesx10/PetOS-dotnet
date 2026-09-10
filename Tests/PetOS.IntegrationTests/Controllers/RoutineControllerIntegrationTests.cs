using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PetOS.Data;
using PetOS.Dto.Pet;
using PetOS.Dto.Routine;
using PetOS.IntegrationTests.Infrastructure;

namespace PetOS.IntegrationTests.Controllers;

[Collection("PetOS Integration Collection")]
public class RoutineControllerIntegrationTests
{
    private readonly PetOsWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public RoutineControllerIntegrationTests(
        PetOsWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private void ResetDatabase()
    {
        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    private async Task<long> CreatePetAsync()
    {
        var pet = new PetCreateDto
        {
            Name = "Luna",
            Species = "Gato",
            Breed = "Siamês",
            BirthDate = new DateTime(2023, 3, 15),
            Gender = "Fêmea",
            weight = 5
        };

        var response =
            await _client.PostAsJsonAsync("/api/Pet", pet);

        var json = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);

        return document.RootElement
            .GetProperty("data")
            .GetProperty("id")
            .GetInt64();
    }

    [Fact]
    public async Task CreateAndGet_ValidRoutine_ReturnsCreatedRoutine()
    {
        // Arrange
        ResetDatabase();

        var petId = await CreatePetAsync();

        var dto = new RoutineCreateDto
        {
            PetId = petId,
            Type = "Alimentação",
            Description = "Ração pela manhã",
            Date = DateTime.UtcNow,
            Notes = "Sem alterações"
        };

        var createResponse =
            await _client.PostAsJsonAsync("/api/Routine", dto);

        var json =
            await createResponse.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);

        var routineId = document.RootElement
            .GetProperty("data")
            .GetProperty("id")
            .GetInt64();

        // Act
        var response =
            await _client.GetAsync($"/api/Routine/{routineId}");

        // Assert
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();

        Assert.Contains("Alimentação", body);
    }

    [Fact]
    public async Task GetById_RoutineDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        ResetDatabase();

        // Act
        var response =
            await _client.GetAsync("/api/Routine/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task GetByPetId_PetDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        ResetDatabase();

        // Act
        var response =
            await _client.GetAsync("/api/Routine/pet/999999");

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    [Fact]
    public async Task Create_PetDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        ResetDatabase();

        var dto = new RoutineCreateDto
        {
            PetId = 999999,
            Type = "Alimentação",
            Description = "Ração pela manhã",
            Date = DateTime.UtcNow,
            Notes = "Teste"
        };

        // Act
        var response =
            await _client.PostAsJsonAsync("/api/Routine", dto);

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );

        var body =
            await response.Content.ReadAsStringAsync();

        Assert.Contains("Pet não encontrado", body);
    }
}