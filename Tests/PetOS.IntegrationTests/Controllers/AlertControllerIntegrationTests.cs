using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PetOS.Data;
using PetOS.Dto.Alert;
using PetOS.Dto.Pet;
using PetOS.Dto.Vaccine;
using PetOS.IntegrationTests.Infrastructure;

namespace PetOS.IntegrationTests.Controllers;

[Collection("PetOS Integration Collection")]
public class AlertControllerIntegrationTests
{
    private readonly PetOsWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AlertControllerIntegrationTests(
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

    private async Task<long> CreatePetAsync(string name)
    {
        var pet = new PetCreateDto
        {
            Name = name,
            Species = "Cachorro",
            Breed = "Vira-lata",
            BirthDate = new DateTime(2020, 4, 10),
            Gender = "Fêmea",
            weight = 12
        };

        var response =
            await _client.PostAsJsonAsync("/api/Pet", pet);

        var json =
            await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);

        return document.RootElement
            .GetProperty("data")
            .GetProperty("id")
            .GetInt64();
    }

    [Fact]
    public async Task CreateAndGet_ValidAlert_ReturnsCreatedAlert()
    {
        // Arrange
        ResetDatabase();

        var petId = await CreatePetAsync("Mel");

        var dto = new AlertCreateDto
        {
            PetId = petId,
            VaccineId = null,
            Message = "Hora do medicamento",
            AlertDate = DateTime.UtcNow
        };

        var createResponse =
            await _client.PostAsJsonAsync("/api/Alert", dto);

        var json =
            await createResponse.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);

        var alertId = document.RootElement
            .GetProperty("data")
            .GetProperty("id")
            .GetInt64();

        // Act
        var response =
            await _client.GetAsync($"/api/Alert/{alertId}");

        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            createResponse.StatusCode
        );

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode
        );

        var body =
            await response.Content.ReadAsStringAsync();

        Assert.Contains(
            "Hora do medicamento",
            body
        );
    }

    [Fact]
    public async Task GetById_AlertDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        ResetDatabase();

        // Act
        var response =
            await _client.GetAsync("/api/Alert/999999");

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

        var dto = new AlertCreateDto
        {
            PetId = 999999,
            VaccineId = null,
            Message = "Hora do medicamento",
            AlertDate = DateTime.UtcNow
        };

        // Act
        var response =
            await _client.PostAsJsonAsync("/api/Alert", dto);

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );

        var body =
            await response.Content.ReadAsStringAsync();

        Assert.Contains(
            "Pet não encontrado",
            body
        );
    }

    [Fact]
    public async Task Create_VaccineDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        ResetDatabase();

        var petId = await CreatePetAsync("Mel");

        var dto = new AlertCreateDto
        {
            PetId = petId,
            VaccineId = 999999,
            Message = "Alerta da vacina",
            AlertDate = DateTime.UtcNow
        };

        // Act
        var response =
            await _client.PostAsJsonAsync("/api/Alert", dto);

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );

        var body =
            await response.Content.ReadAsStringAsync();

        Assert.Contains(
            "Vacina não encontrada",
            body
        );
    }

    [Fact]
    public async Task Create_VaccineBelongsToAnotherPet_ReturnsBadRequest()
    {
        // Arrange
        ResetDatabase();

        var thorId =
            await CreatePetAsync("Thor");

        var lunaId =
            await CreatePetAsync("Luna");

        var vaccineDto = new VaccineCreateDto
        {
            PetId = lunaId,
            Name = "V10",
            Manufacturer = "Fabricante",
            ApplicationDate = DateTime.UtcNow,
            NextDueDate = DateTime.UtcNow.AddYears(1),
            Dose = "Primeira dose"
        };

        var vaccineResponse =
            await _client.PostAsJsonAsync(
                "/api/Vaccine",
                vaccineDto
            );

        var vaccineJson =
            await vaccineResponse.Content.ReadAsStringAsync();

        using var vaccineDocument =
            JsonDocument.Parse(vaccineJson);

        var vaccineId = vaccineDocument.RootElement
            .GetProperty("data")
            .GetProperty("id")
            .GetInt64();

        var alertDto = new AlertCreateDto
        {
            PetId = thorId,
            VaccineId = vaccineId,
            Message = "Alerta incompatível",
            AlertDate = DateTime.UtcNow
        };

        // Act
        var response =
            await _client.PostAsJsonAsync(
                "/api/Alert",
                alertDto
            );

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode
        );

        var body =
            await response.Content.ReadAsStringAsync();

        Assert.Contains(
            "A vacina informada não pertence ao pet informado",
            body
        );
    }
}