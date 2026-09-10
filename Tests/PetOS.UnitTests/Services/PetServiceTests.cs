using Moq;
using PetOS.Dto.Pet;
using PetOS.Models;
using PetOS.Repositories.Interfaces;
using PetOS.Services;

namespace PetOS.UnitTests.Services;

public class PetServiceTests
{
    [Fact]
    public async Task GetByIdAsync_PetExists_ReturnsPetResponseDto()
    {
        // Arrange
        var repositoryMock = new Mock<IPetRepository>();

        var pet = new Pet
        {
            Id = 1,
            Name = "Thor",
            Species = "Cachorro",
            Breed = "Golden Retriever",
            BirthDate = new DateTime(2022, 5, 10),
            Gender = "Macho",
            Weight = 30,
            CreatedAt = new DateTime(2026, 1, 1)
        };

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(pet);

        var service = new PetService(repositoryMock.Object);

        // Act
        var result = await service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Thor", result.Name);
        Assert.Equal("Cachorro", result.Species);
        Assert.Equal("Golden Retriever", result.Breed);
        Assert.Equal(30, result.weight);

        repositoryMock.Verify(
            repository => repository.GetByIdAsync(1),
            Times.Once
        );
    }

    [Fact]
    public async Task GetByIdAsync_PetDoesNotExist_ReturnsNull()
    {
        // Arrange
        var repositoryMock = new Mock<IPetRepository>();

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(999))
            .ReturnsAsync((Pet?)null);

        var service = new PetService(repositoryMock.Object);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);

        repositoryMock.Verify(
            repository => repository.GetByIdAsync(999),
            Times.Once
        );
    }

    [Fact]
    public async Task GetAllSync_PetsExist_ReturnsPetResponseDtoList()
    {
        // Arrange
        var repositoryMock = new Mock<IPetRepository>();

        var pets = new List<Pet>
        {
            new Pet
            {
                Id = 1,
                Name = "Thor",
                Species = "Cachorro",
                Breed = "Golden Retriever",
                BirthDate = new DateTime(2022, 5, 10),
                Gender = "Macho",
                Weight = 30,
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new Pet
            {
                Id = 2,
                Name = "Luna",
                Species = "Gato",
                Breed = "Siamês",
                BirthDate = new DateTime(2023, 3, 15),
                Gender = "Fêmea",
                Weight = 5,
                CreatedAt = new DateTime(2026, 1, 2)
            }
        };

        repositoryMock
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(pets);

        var service = new PetService(repositoryMock.Object);

        // Act
        var result = await service.GetAllSync();
        var resultList = result.ToList();

        // Assert
        Assert.Equal(2, resultList.Count);

        Assert.Equal("Thor", resultList[0].Name);
        Assert.Equal("Cachorro", resultList[0].Species);

        Assert.Equal("Luna", resultList[1].Name);
        Assert.Equal("Gato", resultList[1].Species);

        repositoryMock.Verify(
            repository => repository.GetAllAsync(),
            Times.Once
        );
    }

    [Fact]
    public async Task GetAllSync_NoPets_ReturnsEmptyList()
    {
        // Arrange
        var repositoryMock = new Mock<IPetRepository>();

        repositoryMock
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(new List<Pet>());

        var service = new PetService(repositoryMock.Object);

        // Act
        var result = await service.GetAllSync();

        // Assert
        Assert.Empty(result);

        repositoryMock.Verify(
            repository => repository.GetAllAsync(),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateAsync_ValidPet_ReturnsCreatedPetResponseDto()
    {
        // Arrange
        var repositoryMock = new Mock<IPetRepository>();

        var dto = new PetCreateDto
        {
            Name = "Bob",
            Species = "Cachorro",
            Breed = "Labrador",
            BirthDate = new DateTime(2021, 8, 20),
            Gender = "Macho",
            weight = 25
        };

        repositoryMock
            .Setup(repository => repository.AddAsync(It.IsAny<Pet>()))
            .Callback<Pet>(pet => pet.Id = 10)
            .Returns(Task.CompletedTask);

        var service = new PetService(repositoryMock.Object);

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("Bob", result.Name);
        Assert.Equal("Cachorro", result.Species);
        Assert.Equal("Labrador", result.Breed);
        Assert.Equal("Macho", result.Gender);
        Assert.Equal(25, result.weight);

        repositoryMock.Verify(
            repository => repository.AddAsync(
                It.Is<Pet>(pet =>
                    pet.Name == "Bob" &&
                    pet.Species == "Cachorro" &&
                    pet.Breed == "Labrador" &&
                    pet.Gender == "Macho" &&
                    pet.Weight == 25
                )
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateAsync_PetExists_UpdatesPet()
    {
        // Arrange
        var repositoryMock = new Mock<IPetRepository>();

        var pet = new Pet
        {
            Id = 1,
            Name = "Thor",
            Species = "Cachorro",
            Breed = "Golden Retriever",
            BirthDate = new DateTime(2022, 5, 10),
            Gender = "Macho",
            Weight = 30
        };

        var dto = new PetCreateDto
        {
            Name = "Thor Atualizado",
            Species = "Cachorro",
            Breed = "Golden Retriever",
            BirthDate = new DateTime(2022, 5, 10),
            Gender = "Macho",
            weight = 32
        };

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(pet);

        var service = new PetService(repositoryMock.Object);

        // Act
        await service.UpdateAsync(1, dto);

        // Assert
        repositoryMock.Verify(
            repository => repository.UpdateAsync(
                It.Is<Pet>(updatedPet =>
                    updatedPet.Id == 1 &&
                    updatedPet.Name == "Thor Atualizado" &&
                    updatedPet.Weight == 32
                )
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateAsync_PetDoesNotExist_DoesNotUpdatePet()
    {
        // Arrange
        var repositoryMock = new Mock<IPetRepository>();

        var dto = new PetCreateDto
        {
            Name = "Pet",
            Species = "Cachorro",
            Breed = "Labrador",
            BirthDate = new DateTime(2020, 1, 1),
            Gender = "Macho",
            weight = 20
        };

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(999))
            .ReturnsAsync((Pet?)null);

        var service = new PetService(repositoryMock.Object);

        // Act
        await service.UpdateAsync(999, dto);

        // Assert
        repositoryMock.Verify(
            repository => repository.UpdateAsync(It.IsAny<Pet>()),
            Times.Never
        );
    }

    [Fact]
    public async Task DeleteAsync_ValidId_CallsRepositoryDeleteOnce()
    {
        // Arrange
        var repositoryMock = new Mock<IPetRepository>();

        repositoryMock
            .Setup(repository => repository.DeleteAsync(1))
            .Returns(Task.CompletedTask);

        var service = new PetService(repositoryMock.Object);

        // Act
        await service.DeleteAsync(1);

        // Assert
        repositoryMock.Verify(
            repository => repository.DeleteAsync(1),
            Times.Once
        );
    }

    [Fact]
    public async Task GetBySpecieAsync_SpeciesExists_ReturnsMatchingPets()
    {
        // Arrange
        var repositoryMock = new Mock<IPetRepository>();

        var pets = new List<Pet>
        {
            new Pet
            {
                Id = 1,
                Name = "Luna",
                Species = "Gato",
                Breed = "Siamês",
                BirthDate = new DateTime(2023, 3, 15),
                Gender = "Fêmea",
                Weight = 5
            }
        };

        repositoryMock
            .Setup(repository => repository.GetBySpeciesAsync("Gato"))
            .ReturnsAsync(pets);

        var service = new PetService(repositoryMock.Object);

        // Act
        var result = await service.GetBySpecieAsync("Gato");
        var resultList = result.ToList();

        // Assert
        Assert.Single(resultList);
        Assert.Equal("Luna", resultList[0].Name);
        Assert.Equal("Gato", resultList[0].Species);

        repositoryMock.Verify(
            repository => repository.GetBySpeciesAsync("Gato"),
            Times.Once
        );
    }

    [Fact]
    public async Task GetByNameAsync_NameExists_ReturnsMatchingPets()
    {
        // Arrange
        var repositoryMock = new Mock<IPetRepository>();

        var pets = new List<Pet>
        {
            new Pet
            {
                Id = 1,
                Name = "Thor",
                Species = "Cachorro",
                Breed = "Golden Retriever",
                BirthDate = new DateTime(2022, 5, 10),
                Gender = "Macho",
                Weight = 30
            }
        };

        repositoryMock
            .Setup(repository => repository.GetByNameAsync("Thor"))
            .ReturnsAsync(pets);

        var service = new PetService(repositoryMock.Object);

        // Act
        var result = await service.GetByNameAsync("Thor");
        var resultList = result.ToList();

        // Assert
        Assert.Single(resultList);
        Assert.Equal("Thor", resultList[0].Name);

        repositoryMock.Verify(
            repository => repository.GetByNameAsync("Thor"),
            Times.Once
        );
    }
}