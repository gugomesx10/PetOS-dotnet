using System.ComponentModel.DataAnnotations;
using PetOS.Models;

namespace PetOS.UnitTests.Domain;

public class ModelValidationTests
{
    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(
            model,
            new ValidationContext(model),
            results,
            validateAllProperties: true
        );

        return results;
    }

    [Fact]
    public void ValidatePet_ValidData_ReturnsNoErrors()
    {
        // Arrange
        var pet = new Pet
        {
            Name = "Thor",
            Species = "Cachorro",
            Breed = "Golden Retriever",
            BirthDate = new DateTime(2022, 5, 10),
            Gender = "Macho",
            Weight = 30,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = Validate(pet);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void ValidatePet_EmptyName_ReturnsValidationError()
    {
        // Arrange
        var pet = new Pet
        {
            Name = "",
            Species = "Cachorro",
            Breed = "Labrador",
            BirthDate = new DateTime(2022, 5, 10),
            Gender = "Macho",
            Weight = 25
        };

        // Act
        var result = Validate(pet);

        // Assert
        Assert.Contains(
            result,
            error => error.MemberNames.Contains(nameof(Pet.Name))
        );
    }

    [Fact]
    public void ValidatePet_WeightBelowMinimum_ReturnsValidationError()
    {
        // Arrange
        var pet = new Pet
        {
            Name = "Thor",
            Species = "Cachorro",
            Breed = "Labrador",
            BirthDate = new DateTime(2022, 5, 10),
            Gender = "Macho",
            Weight = 0
        };

        // Act
        var result = Validate(pet);

        // Assert
        Assert.Contains(
            result,
            error => error.MemberNames.Contains(nameof(Pet.Weight))
        );
    }

    [Fact]
    public void ValidatePet_NameAboveMaximumLength_ReturnsValidationError()
    {
        // Arrange
        var pet = new Pet
        {
            Name = new string('A', 101),
            Species = "Cachorro",
            Breed = "Labrador",
            BirthDate = new DateTime(2022, 5, 10),
            Gender = "Macho",
            Weight = 20
        };

        // Act
        var result = Validate(pet);

        // Assert
        Assert.Contains(
            result,
            error => error.MemberNames.Contains(nameof(Pet.Name))
        );
    }

    [Fact]
    public void ValidateVaccine_ValidData_ReturnsNoErrors()
    {
        // Arrange
        var vaccine = new Vaccine
        {
            PetId = 1,
            Name = "Antirrábica",
            Manufacturer = "Fabricante",
            ApplicationDate = new DateTime(2026, 1, 10),
            NextDueDate = new DateTime(2027, 1, 10),
            Dose = "Primeira dose",
            CreateAt = DateTime.UtcNow
        };

        // Act
        var result = Validate(vaccine);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void ValidateVaccine_EmptyName_ReturnsValidationError()
    {
        // Arrange
        var vaccine = new Vaccine
        {
            PetId = 1,
            Name = "",
            Manufacturer = "Fabricante",
            ApplicationDate = new DateTime(2026, 1, 10),
            NextDueDate = new DateTime(2027, 1, 10),
            Dose = "Primeira dose"
        };

        // Act
        var result = Validate(vaccine);

        // Assert
        Assert.Contains(
            result,
            error => error.MemberNames.Contains(nameof(Vaccine.Name))
        );
    }

    [Fact]
    public void ValidateRoutineRecord_ValidData_ReturnsNoErrors()
    {
        // Arrange
        var routine = new RoutineRecord
        {
            PetId = 1,
            Type = "Alimentação",
            Description = "Ração pela manhã",
            Date = DateTime.UtcNow,
            Notes = "Sem alterações",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = Validate(routine);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void ValidateRoutineRecord_DescriptionAboveMaximumLength_ReturnsValidationError()
    {
        // Arrange
        var routine = new RoutineRecord
        {
            PetId = 1,
            Type = "Alimentação",
            Description = new string('A', 256),
            Date = DateTime.UtcNow,
            Notes = "Teste"
        };

        // Act
        var result = Validate(routine);

        // Assert
        Assert.Contains(
            result,
            error => error.MemberNames.Contains(nameof(RoutineRecord.Description))
        );
    }

    [Fact]
    public void ValidateAlert_ValidData_ReturnsNoErrors()
    {
        // Arrange
        var alert = new Alert
        {
            PetId = 1,
            Message = "Vacina próxima do vencimento",
            AlertDate = DateTime.UtcNow,
            IsRead = 0,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = Validate(alert);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void ValidateAlert_EmptyMessage_ReturnsValidationError()
    {
        // Arrange
        var alert = new Alert
        {
            PetId = 1,
            Message = "",
            AlertDate = DateTime.UtcNow,
            IsRead = 0,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = Validate(alert);

        // Assert
        Assert.Contains(
            result,
            error => error.MemberNames.Contains(nameof(Alert.Message))
        );
    }
}