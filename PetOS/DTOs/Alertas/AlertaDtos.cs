using System.ComponentModel.DataAnnotations;
using PetOS.Models;

namespace PetOS.DTOs.Alertas;

public record AlertaResponseDto(
    int Id,
    int PetId,
    int? RotinaId,
    string Titulo,
    string Mensagem,
    DateTime DataAlerta,
    AlertaStatus Status);

public record CreateAlertaDto(
    [property: Required] int PetId,
    int? RotinaId,
    [property: Required, MaxLength(120)] string Titulo,
    [property: Required, MaxLength(600)] string Mensagem,
    DateTime DataAlerta,
    AlertaStatus Status);

public record UpdateAlertaDto(
    int? RotinaId,
    [property: Required, MaxLength(120)] string Titulo,
    [property: Required, MaxLength(600)] string Mensagem,
    DateTime DataAlerta,
    AlertaStatus Status);

