using System.ComponentModel.DataAnnotations;
using PetOS.Models;

namespace PetOS.DTOs.Rotinas;

public record RotinaResponseDto(
    int Id,
    int PetId,
    string Titulo,
    string? Descricao,
    TimeOnly Horario,
    RotinaTipo Tipo,
    string Frequencia,
    bool Ativa);

public record CreateRotinaDto(
    [property: Required] int PetId,
    [property: Required, MaxLength(120)] string Titulo,
    [property: MaxLength(400)] string? Descricao,
    TimeOnly Horario,
    RotinaTipo Tipo,
    [property: MaxLength(30)] string Frequencia,
    bool Ativa);

public record UpdateRotinaDto(
    [property: Required, MaxLength(120)] string Titulo,
    [property: MaxLength(400)] string? Descricao,
    TimeOnly Horario,
    RotinaTipo Tipo,
    [property: MaxLength(30)] string Frequencia,
    bool Ativa);

