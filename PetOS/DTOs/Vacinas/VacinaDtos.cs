using System.ComponentModel.DataAnnotations;

namespace PetOS.DTOs.Vacinas;

public record VacinaResponseDto(
    int Id,
    int PetId,
    string Nome,
    DateOnly DataAplicacao,
    DateOnly? ProximaDose,
    string? Observacoes);

public record CreateVacinaDto(
    [property: Required] int PetId,
    [property: Required, MaxLength(120)] string Nome,
    DateOnly DataAplicacao,
    DateOnly? ProximaDose,
    [property: MaxLength(400)] string? Observacoes);

public record UpdateVacinaDto(
    [property: Required, MaxLength(120)] string Nome,
    DateOnly DataAplicacao,
    DateOnly? ProximaDose,
    [property: MaxLength(400)] string? Observacoes);

