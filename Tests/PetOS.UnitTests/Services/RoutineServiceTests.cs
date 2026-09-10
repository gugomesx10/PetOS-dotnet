using Moq;
using PetOS.Dto.Routine;
using PetOS.Models;
using PetOS.Repositories.Interfaces;
using PetOS.Services;

namespace PetOS.UnitTests.Services;

public class RoutineServiceTests
{
    [Fact]
    public async Task GetByIdAsync_RoutineExists_ReturnsRoutineResponseDto()
    {
        // Arrange
        var repositoryMock = new Mock<IRoutineRepository>();

        var routine = new RoutineRecord
        {
            Id = 1,
            PetId = 1,
            Type = "Alimentação",
            Description = "Ração pela manhã",
            Date = DateTime.UtcNow,
            Notes = "Sem alterações"
        };

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(routine);

        var service = new RoutineService(repositoryMock.Object);

        // Act
        var result = await service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Alimentação", result.Type);
        Assert.Equal("Ração pela manhã", result.Description);

        repositoryMock.Verify(
            repository => repository.GetByIdAsync(1),
            Times.Once
        );
    }

    [Fact]
    public async Task GetByIdAsync_RoutineDoesNotExist_ReturnsNull()
    {
        // Arrange
        var repositoryMock = new Mock<IRoutineRepository>();

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(999))
            .ReturnsAsync((RoutineRecord?)null);

        var service = new RoutineService(repositoryMock.Object);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ValidRoutine_ReturnsCreatedRoutine()
    {
        // Arrange
        var repositoryMock = new Mock<IRoutineRepository>();

        var dto = new RoutineCreateDto
        {
            PetId = 1,
            Type = "Alimentação",
            Description = "Ração pela manhã",
            Date = new DateTime(2026, 9, 10),
            Notes = "Tudo normal"
        };

        repositoryMock
            .Setup(repository => repository.AddAsync(It.IsAny<RoutineRecord>()))
            .Callback<RoutineRecord>(routine => routine.Id = 10)
            .Returns(Task.CompletedTask);

        var service = new RoutineService(repositoryMock.Object);

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.Equal(10, result.Id);
        Assert.Equal(1, result.PetId);
        Assert.Equal("Alimentação", result.Type);

        repositoryMock.Verify(
            repository => repository.AddAsync(
                It.Is<RoutineRecord>(routine =>
                    routine.PetId == 1 &&
                    routine.Type == "Alimentação" &&
                    routine.Description == "Ração pela manhã"
                )
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateAsync_RoutineDoesNotExist_DoesNotUpdateRoutine()
    {
        // Arrange
        var repositoryMock = new Mock<IRoutineRepository>();

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(999))
            .ReturnsAsync((RoutineRecord?)null);

        var dto = new RoutineCreateDto
        {
            PetId = 1,
            Type = "Passeio",
            Description = "Passeio no parque",
            Date = DateTime.UtcNow,
            Notes = "Teste"
        };

        var service = new RoutineService(repositoryMock.Object);

        // Act
        await service.UpdateAsync(999, dto);

        // Assert
        repositoryMock.Verify(
            repository => repository.UpdateAsync(It.IsAny<RoutineRecord>()),
            Times.Never
        );
    }
}