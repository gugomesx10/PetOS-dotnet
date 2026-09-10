using System.Net;
using PetOS.IntegrationTests.Infrastructure;

namespace PetOS.IntegrationTests.HealthChecks;

[Collection("PetOS Integration Collection")]
public class HealthChecksIntegrationTests
{
    private readonly HttpClient _client;

    public HealthChecksIntegrationTests(
        PetOsWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthLive_ApiIsRunning_ReturnsOk()
    {
        // Arrange

        // Act
        var response =
            await _client.GetAsync("/health/live");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode
        );
    }

    [Fact]
    public async Task HealthReady_DatabaseIsAvailable_ReturnsOk()
    {
        // Arrange

        // Act
        var response =
            await _client.GetAsync("/health/ready");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode
        );
    }
}