namespace PetOS.IntegrationTests.Infrastructure;

[CollectionDefinition("PetOS Integration Collection")]
public class PetOsIntegrationCollection
    : ICollectionFixture<PetOsWebApplicationFactory>
{
}