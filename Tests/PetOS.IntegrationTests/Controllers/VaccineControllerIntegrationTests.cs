using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PetOS.Data;
using PetOS.Dto.Pet;
using PetOS.Dto.Vaccine;
using PetOS.IntegrationTests.Infrastructure;

namespace PetOS.IntegrationTests.Controllers;

[Collection("PetOS Integration Collection")]
public class VaccineControllerIntegrationTests
{
    private readonly PetOsWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public VaccineControllerIntegrationTests(
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
            Name = "Thor",
            Species = "Cachorro",
            Breed = "Golden Retriever",
            BirthDate = new DateTime(2022, 5, 10),
            Gender = "Macho",
            weight = 30
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
    public async Task CreateAndGet_ValidVaccine_ReturnsCreatedVaccine()
    {
        // Arrange
        ResetDatabase();

        var petId = await CreatePetAsync();

        var dto = new VaccineCreateDto
        {
            PetId = petId,
            Name = "V10",
            Manufacturer = "Fabricante",
            ApplicationDate = new DateTime(2026, 9, 10),
            NextDueDate = new DateTime(2027, 9, 10),
            Dose = "Primeira dose"
        };

        var createResponse =
            await _client.PostAsJsonAsync("/api/Vaccine", dto);

        var json =
            await createResponse.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);

        var vaccineId = document.RootElement
            .GetProperty("data")
            .GetProperty("id")
            .GetInt64();

        // Act
        var response =
            await _client.GetAsync($"/api/Vaccine/{vaccineId}");

        // Assert
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();

        Assert.Contains("V10", body);
    }

    [Fact]
    public async Task GetById_VaccineDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        ResetDatabase();

        // Act
        var response =
            await _client.GetAsync("/api/Vaccine/999999");

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
            await _client.GetAsync("/api/Vaccine/pet/999999");

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

        var dto = new VaccineCreateDto
        {
            PetId = 999999,
            Name = "V10",
            Manufacturer = "Fabricante",
            ApplicationDate = new DateTime(2026, 9, 10),
            NextDueDate = new DateTime(2027, 9, 10),
            Dose = "Primeira dose"
        };

        // Act
        var response =
            await _client.PostAsJsonAsync("/api/Vaccine", dto);

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