using System.ComponentModel.DataAnnotations;

namespace PetOS.DTOs.Pets;

public record PetResponseDto(
    int Id,
    string Nome,
    string Especie,
    string? Raca,
    DateOnly? DataNascimento,
    string ResponsavelNome,
    string? ResponsavelTelefone,
    DateTime CriadoEm);

public record CreatePetDto(
    [property: Required, MaxLength(120)] string Nome,
    [property: Required, MaxLength(80)] string Especie,
    [property: MaxLength(80)] string? Raca,
    DateOnly? DataNascimento,
    [property: Required, MaxLength(120)] string ResponsavelNome,
    [property: MaxLength(30)] string? ResponsavelTelefone);

public record UpdatePetDto(
    [property: Required, MaxLength(120)] string Nome,
    [property: Required, MaxLength(80)] string Especie,
    [property: MaxLength(80)] string? Raca,
    DateOnly? DataNascimento,
    [property: Required, MaxLength(120)] string ResponsavelNome,
    [property: MaxLength(30)] string? ResponsavelTelefone);

