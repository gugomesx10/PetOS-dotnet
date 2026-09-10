using Moq;
using PetOS.Dto.Vaccine;
using PetOS.Models;
using PetOS.Repositories.Interfaces;
using PetOS.Services;

namespace PetOS.UnitTests.Services;

public class VaccineServiceTests
{
    [Fact]
    public async Task GetByIdAsync_VaccineExists_ReturnsVaccineResponseDto()
    {
        // Arrange
        var repositoryMock = new Mock<IVaccineRepository>();

        var vaccine = new Vaccine
        {
            Id = 1,
            PetId = 1,
            Name = "Antirrábica",
            Manufacturer = "Fabricante",
            ApplicationDate = new DateTime(2026, 1, 10),
            NextDueDate = new DateTime(2027, 1, 10),
            Dose = "Primeira dose"
        };

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(vaccine);

        var service = new VaccineService(repositoryMock.Object);

        // Act
        var result = await service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Antirrábica", result.Name);
        Assert.Equal(1, result.PetId);

        repositoryMock.Verify(
            repository => repository.GetByIdAsync(1),
            Times.Once
        );
    }

    [Fact]
    public async Task GetByIdAsync_VaccineDoesNotExist_ReturnsNull()
    {
        // Arrange
        var repositoryMock = new Mock<IVaccineRepository>();

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(999))
            .ReturnsAsync((Vaccine?)null);

        var service = new VaccineService(repositoryMock.Object);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ValidVaccine_ReturnsCreatedVaccine()
    {
        // Arrange
        var repositoryMock = new Mock<IVaccineRepository>();

        var dto = new VaccineCreateDto
        {
            PetId = 1,
            Name = "V10",
            Manufacturer = "Fabricante",
            ApplicationDate = new DateTime(2026, 2, 1),
            NextDueDate = new DateTime(2027, 2, 1),
            Dose = "Primeira dose"
        };

        repositoryMock
            .Setup(repository => repository.AddAsync(It.IsAny<Vaccine>()))
            .Callback<Vaccine>(vaccine => vaccine.Id = 10)
            .Returns(Task.CompletedTask);

        var service = new VaccineService(repositoryMock.Object);

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.Equal(10, result.Id);
        Assert.Equal("V10", result.Name);
        Assert.Equal(1, result.PetId);

        repositoryMock.Verify(
            repository => repository.AddAsync(
                It.Is<Vaccine>(vaccine =>
                    vaccine.PetId == 1 &&
                    vaccine.Name == "V10" &&
                    vaccine.Manufacturer == "Fabricante"
                )
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateAsync_VaccineDoesNotExist_DoesNotUpdateVaccine()
    {
        // Arrange
        var repositoryMock = new Mock<IVaccineRepository>();

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(999))
            .ReturnsAsync((Vaccine?)null);

        var dto = new VaccineCreateDto
        {
            PetId = 1,
            Name = "V10",
            Manufacturer = "Fabricante",
            ApplicationDate = DateTime.UtcNow,
            NextDueDate = DateTime.UtcNow.AddYears(1),
            Dose = "Reforço"
        };

        var service = new VaccineService(repositoryMock.Object);

        // Act
        await service.UpdateAsync(999, dto);

        // Assert
        repositoryMock.Verify(
            repository => repository.UpdateAsync(It.IsAny<Vaccine>()),
            Times.Never
        );
    }
}