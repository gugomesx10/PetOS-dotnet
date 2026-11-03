using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PetOS.Data;
using PetOS.Dto.Pet;
using PetOS.Dto.Routine;
using PetOS.Dto.Vaccine;
using PetOS.Models;
using PetOS.IntegrationTests.Infrastructure;

namespace PetOS.IntegrationTests.Controllers;

[Collection("PetOS Integration Collection")]
public class ResourceQueryIntegrationTests
{
    private readonly PetOsWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ResourceQueryIntegrationTests(
        PetOsWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateAuthenticatedClient();
    }

    private void ResetDatabase()
    {
        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    private async Task<long> CreatePetAsync(
        string name,
        string species = "Cachorro")
    {
        var response = await _client.PostAsJsonAsync(
            "/api/Pet",
            new PetCreateDto
            {
                Name = name,
                Species = species,
                Breed = "Vira-lata",
                BirthDate = new DateTime(2022, 1, 1),
                Gender = "Macho",
                weight = 12
            });

        response.EnsureSuccessStatusCode();

        using var json = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        return json.RootElement
            .GetProperty("data")
            .GetProperty("id")
            .GetInt64();
    }

    [Fact]
    public async Task Pets_PaginationAndSort_ReturnsPageMetadataAndLinks()
    {
        ResetDatabase();

        await CreatePetAsync("Zeca");
        await CreatePetAsync("Ana");
        await CreatePetAsync("Bela");

        var response = await _client.GetAsync(
            "/api/Pet?page=2&pageSize=1&sortBy=name");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        var root = json.RootElement;

        Assert.Equal(
            3,
            root.GetProperty("pagination")
                .GetProperty("totalItems")
                .GetInt32());

        Assert.Equal(
            2,
            root.GetProperty("pagination")
                .GetProperty("page")
                .GetInt32());

        Assert.Single(
            root.GetProperty("data").EnumerateArray());

        Assert.Equal(
            "Bela",
            root.GetProperty("data")[0]
                .GetProperty("name")
                .GetString());

        Assert.Contains(
            root.GetProperty("links").EnumerateArray(),
            link => link.GetProperty("rel").GetString() == "next");

        Assert.Contains(
            root.GetProperty("links").EnumerateArray(),
            link => link.GetProperty("rel").GetString() == "previous");
    }

    [Fact]
    public async Task Pets_FilterBySpeciesAndName_ReturnsMatchingAnimal()
    {
        ResetDatabase();

        await CreatePetAsync("Luna", "Gato");
        await CreatePetAsync("Luna", "Cachorro");

        var response = await _client.GetAsync(
            "/api/Pet?species=Gato&name=Lu");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        Assert.Single(
            json.RootElement
                .GetProperty("data")
                .EnumerateArray());

        Assert.Equal(
            "Gato",
            json.RootElement
                .GetProperty("data")[0]
                .GetProperty("species")
                .GetString());
    }

    [Fact]
    public async Task Pet_ById_ReturnsHateoasLinks()
    {
        ResetDatabase();

        var petId = await CreatePetAsync("Thor");

        var response = await _client.GetAsync(
            $"/api/Pet/{petId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        Assert.Equal(
            petId,
            json.RootElement
                .GetProperty("data")
                .GetProperty("id")
                .GetInt64());

        Assert.Contains(
            json.RootElement.GetProperty("links").EnumerateArray(),
            link =>
                link.GetProperty("rel").GetString() == "self" &&
                link.GetProperty("href").GetString() == $"/api/Pet/{petId}");
    }

    [Fact]
    public async Task Vaccine_FilterByPet_ReturnsOnlyItsVaccines()
    {
        ResetDatabase();

        var firstPet = await CreatePetAsync("Thor");
        var secondPet = await CreatePetAsync("Mel");

        async Task CreateVaccine(long petId, string name)
        {
            var response = await _client.PostAsJsonAsync(
                "/api/Vaccine",
                new VaccineCreateDto
                {
                    PetId = petId,
                    Name = name,
                    Manufacturer = "Laboratório",
                    ApplicationDate = new DateTime(2026, 1, 1),
                    NextDueDate = new DateTime(2027, 1, 1),
                    Dose = "Primeira dose"
                });

            response.EnsureSuccessStatusCode();
        }

        await CreateVaccine(firstPet, "V10");
        await CreateVaccine(secondPet, "Antirrábica");

        var result = await _client.GetAsync(
            $"/api/Vaccine?petId={firstPet}");

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        using var json = JsonDocument.Parse(
            await result.Content.ReadAsStringAsync());

        Assert.Single(
            json.RootElement.GetProperty("data").EnumerateArray());

        Assert.Equal(
            "V10",
            json.RootElement
                .GetProperty("data")[0]
                .GetProperty("name")
                .GetString());
    }

    [Fact]
    public async Task Routine_FilterByType_ReturnsMatchingRoutine()
    {
        ResetDatabase();

        var petId = await CreatePetAsync("Luna");

        foreach (var type in new[] { "Alimentação", "Passeio" })
        {
            var result = await _client.PostAsJsonAsync(
                "/api/Routine",
                new RoutineCreateDto
                {
                    PetId = petId,
                    Type = type,
                    Description = "Cuidado diário",
                    Date = DateTime.UtcNow,
                    Notes = "Teste"
                });

            result.EnsureSuccessStatusCode();
        }

        var response = await _client.GetAsync(
            "/api/Routine?type=PASSEIO");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        Assert.Single(
            json.RootElement.GetProperty("data").EnumerateArray());
    }

    [Fact]
    public async Task Alert_FilterByUnread_ReturnsOnlyUnreadAlerts()
    {
        ResetDatabase();

        var petId = await CreatePetAsync("Mel");

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            context.Alerts.AddRange(
                new Alert
                {
                    PetId = petId,
                    Message = "Pendente",
                    AlertDate = DateTime.UtcNow,
                    IsRead = 0
                },
                new Alert
                {
                    PetId = petId,
                    Message = "Concluído",
                    AlertDate = DateTime.UtcNow,
                    IsRead = 1
                });

            await context.SaveChangesAsync();
        }

        var response = await _client.GetAsync(
            "/api/Alert?isRead=false");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        Assert.Single(
            json.RootElement.GetProperty("data").EnumerateArray());

        Assert.Equal(
            "Pendente",
            json.RootElement
                .GetProperty("data")[0]
                .GetProperty("message")
                .GetString());
    }

    [Fact]
    public async Task InvalidSort_ReturnsProblemDetailsBadRequest()
    {
        ResetDatabase();

        var response = await _client.GetAsync(
            "/api/Pet?sortBy=inexistente");

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);

        using var json = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        Assert.Equal(
            400,
            json.RootElement
                .GetProperty("status")
                .GetInt32());

        Assert.True(
            json.RootElement.TryGetProperty("traceId", out _));
    }

    [Fact]
    public async Task InvalidPage_ReturnsBadRequest()
    {
        ResetDatabase();

        var response = await _client.GetAsync(
            "/api/Pet?page=0");

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }
}