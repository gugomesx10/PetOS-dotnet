using Moq;
using PetOS.Dto.Common;
using PetOS.Dto.Pet;
using PetOS.Dto.PetActivity;
using PetOS.Models;
using PetOS.Repositories.Interfaces;
using PetOS.Services;
using PetOS.Services.Interfaces;

namespace PetOS.UnitTests.Services;

public class PetActivityServiceTests
{
    private const string ValidId = "507f1f77bcf86cd799439011";

    private readonly Mock<IPetActivityRepository> _repositoryMock = new();
    private readonly Mock<IPetService> _petServiceMock = new();

    private PetActivityService CreateService()
    {
        return new PetActivityService(
            _repositoryMock.Object,
            _petServiceMock.Object
        );
    }

    [Fact]
    public async Task CreateAsync_ValidPet_InsertsActivity()
    {
        // Arrange
        var dto = new PetActivityCreateDto
        {
            PetId = 1,
            Type = "Passeio",
            Description = "Caminhada no parque",
            Metadata = new Dictionary<string, string>
            {
                ["durationMinutes"] = "30"
            }
        };

        _petServiceMock
            .Setup(service => service.GetByIdAsync(1))
            .ReturnsAsync(new PetResponseDto
            {
                Id = 1,
                Name = "Thor"
            });

        _repositoryMock
            .Setup(repository =>
                repository.InsertAsync(It.IsAny<PetActivity>()))
            .Returns(Task.CompletedTask);

        var service = CreateService();

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.PetId);
        Assert.Equal("Passeio", result.Type);
        Assert.Equal("Caminhada no parque", result.Description);
        Assert.Equal("30", result.Metadata["durationMinutes"]);

        _repositoryMock.Verify(
            repository => repository.InsertAsync(
                It.Is<PetActivity>(activity =>
                    activity.PetId == 1 &&
                    activity.Type == "Passeio"
                )
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateAsync_PetDoesNotExist_ThrowsException()
    {
        // Arrange
        var dto = new PetActivityCreateDto
        {
            PetId = 999,
            Type = "Passeio",
            Description = "Atividade de teste"
        };

        _petServiceMock
            .Setup(service => service.GetByIdAsync(999))
            .ReturnsAsync((PetResponseDto?)null);

        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.CreateAsync(dto)
        );

        _repositoryMock.Verify(
            repository => repository.InsertAsync(
                It.IsAny<PetActivity>()
            ),
            Times.Never
        );
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsActivity()
    {
        // Arrange
        var activity = new PetActivity
        {
            Id = ValidId,
            PetId = 1,
            Type = "Consulta",
            Description = "Consulta veterinária"
        };

        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(ValidId))
            .ReturnsAsync(activity);

        var service = CreateService();

        // Act
        var result = await service.GetByIdAsync(ValidId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ValidId, result.Id);
        Assert.Equal("Consulta", result.Type);

        _repositoryMock.Verify(
            repository => repository.GetByIdAsync(ValidId),
            Times.Once
        );
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ThrowsException()
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.GetByIdAsync("id-invalido")
        );
    }

    [Fact]
    public async Task GetPagedAsync_ValidQuery_ReturnsPagedResult()
    {
        // Arrange
        var query = new PetActivityQuery
        {
            Page = 1,
            PageSize = 10,
            PetId = 1
        };

        var activities = new List<PetActivity>
        {
            new()
            {
                Id = ValidId,
                PetId = 1,
                Type = "Passeio",
                Description = "Caminhada"
            }
        };

        var pagedResult = new PagedResult<PetActivity>(
            activities,
            1,
            10,
            1
        );

        _repositoryMock
            .Setup(repository => repository.GetPagedAsync(query))
            .ReturnsAsync(pagedResult);

        var service = CreateService();

        // Act
        var result = await service.GetPagedAsync(query);

        // Assert
        Assert.Single(result.Items);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public async Task UpdateAsync_ExistingActivity_UpdatesSuccessfully()
    {
        // Arrange
        var existingActivity = new PetActivity
        {
            Id = ValidId,
            PetId = 1,
            Type = "Passeio",
            Description = "Caminhada curta"
        };

        var dto = new PetActivityCreateDto
        {
            PetId = 1,
            Type = "Passeio",
            Description = "Caminhada de 60 minutos"
        };

        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(ValidId))
            .ReturnsAsync(existingActivity);

        _petServiceMock
            .Setup(service => service.GetByIdAsync(1))
            .ReturnsAsync(new PetResponseDto
            {
                Id = 1,
                Name = "Thor"
            });

        _repositoryMock
            .Setup(repository => repository.ReplaceAsync(
                It.IsAny<PetActivity>()))
            .ReturnsAsync(true);

        var service = CreateService();

        // Act
        var updated = await service.UpdateAsync(ValidId, dto);

        // Assert
        Assert.True(updated);
        Assert.Equal(
            "Caminhada de 60 minutos",
            existingActivity.Description
        );

        _repositoryMock.Verify(
            repository => repository.ReplaceAsync(
                It.IsAny<PetActivity>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateAsync_ActivityDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var dto = new PetActivityCreateDto
        {
            PetId = 1,
            Type = "Passeio",
            Description = "Atualização"
        };

        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(ValidId))
            .ReturnsAsync((PetActivity?)null);

        var service = CreateService();

        // Act
        var updated = await service.UpdateAsync(ValidId, dto);

        // Assert
        Assert.False(updated);

        _repositoryMock.Verify(
            repository => repository.ReplaceAsync(
                It.IsAny<PetActivity>()
            ),
            Times.Never
        );
    }

    [Fact]
    public async Task DeleteAsync_ExistingActivity_ReturnsTrue()
    {
        // Arrange
        _repositoryMock
            .Setup(repository => repository.DeleteAsync(ValidId))
            .ReturnsAsync(true);

        var service = CreateService();

        // Act
        var deleted = await service.DeleteAsync(ValidId);

        // Assert
        Assert.True(deleted);

        _repositoryMock.Verify(
            repository => repository.DeleteAsync(ValidId),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteAsync_InvalidId_ThrowsException()
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.DeleteAsync("id-invalido")
        );
    }
}