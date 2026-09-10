using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PetOS.Data;
using PetOS.Dto.Pet;
using PetOS.IntegrationTests.Infrastructure;

namespace PetOS.IntegrationTests.Controllers;

[Collection("PetOS Integration Collection")]
public class PetControllerIntegrationTests
{
    private readonly PetOsWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public PetControllerIntegrationTests(
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

    [Fact]
    public async Task GetAll_NoPets_ReturnsNoContent()
    {
        // Arrange
        ResetDatabase();

        // Act
        var response = await _client.GetAsync("/api/Pet");

        // Assert
        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode
        );
    }

    [Fact]
    public async Task GetById_PetDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        ResetDatabase();

        // Act
        var response =
            await _client.GetAsync("/api/Pet/999999");

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    [Fact]
    public async Task Create_ValidPet_ReturnsCreated()
    {
        // Arrange
        ResetDatabase();

        var dto = new PetCreateDto
        {
            Name = "Thor",
            Species = "Cachorro",
            Breed = "Golden Retriever",
            BirthDate = new DateTime(2022, 5, 10),
            Gender = "Macho",
            weight = 30
        };

        // Act
        var response =
            await _client.PostAsJsonAsync("/api/Pet", dto);

        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode
        );
    }

    [Fact]
    public async Task CreateAndGet_ValidPet_ReturnsCreatedPet()
    {
        // Arrange
        ResetDatabase();

        var dto = new PetCreateDto
        {
            Name = "Luna",
            Species = "Gato",
            Breed = "Siamês",
            BirthDate = new DateTime(2023, 3, 15),
            Gender = "Fêmea",
            weight = 5
        };

        var createResponse =
            await _client.PostAsJsonAsync("/api/Pet", dto);

        var json =
            await createResponse.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);

        var id = document.RootElement
            .GetProperty("data")
            .GetProperty("id")
            .GetInt64();

        // Act
        var response =
            await _client.GetAsync($"/api/Pet/{id}");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode
        );

        var responseBody =
            await response.Content.ReadAsStringAsync();

        Assert.Contains("Luna", responseBody);
        Assert.Contains("Gato", responseBody);
    }

    [Fact]
    public async Task Update_ExistingPet_ReturnsOk()
    {
        // Arrange
        ResetDatabase();

        var createDto = new PetCreateDto
        {
            Name = "Bob",
            Species = "Cachorro",
            Breed = "Labrador",
            BirthDate = new DateTime(2021, 8, 20),
            Gender = "Macho",
            weight = 25
        };

        var createResponse =
            await _client.PostAsJsonAsync("/api/Pet", createDto);

        var json =
            await createResponse.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);

        var id = document.RootElement
            .GetProperty("data")
            .GetProperty("id")
            .GetInt64();

        var updateDto = new PetCreateDto
        {
            Name = "Bob Atualizado",
            Species = "Cachorro",
            Breed = "Labrador",
            BirthDate = new DateTime(2021, 8, 20),
            Gender = "Macho",
            weight = 28
        };

        // Act
        var response =
            await _client.PutAsJsonAsync(
                $"/api/Pet/{id}",
                updateDto
            );

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode
        );
    }

    [Fact]
    public async Task Delete_ExistingPet_ReturnsOk()
    {
        // Arrange
        ResetDatabase();

        var dto = new PetCreateDto
        {
            Name = "Mel",
            Species = "Cachorro",
            Breed = "Vira-lata",
            BirthDate = new DateTime(2020, 4, 10),
            Gender = "Fêmea",
            weight = 12
        };

        var createResponse =
            await _client.PostAsJsonAsync("/api/Pet", dto);

        var json =
            await createResponse.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);

        var id = document.RootElement
            .GetProperty("data")
            .GetProperty("id")
            .GetInt64();

        // Act
        var response =
            await _client.DeleteAsync($"/api/Pet/{id}");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode
        );

        var getResponse =
            await _client.GetAsync($"/api/Pet/{id}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            getResponse.StatusCode
        );
    }
}