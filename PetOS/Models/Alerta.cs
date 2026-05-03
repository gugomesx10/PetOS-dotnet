using System.ComponentModel.DataAnnotations;

namespace PetOS.Models;

public class Alerta
{
    public int Id { get; set; }

    [Required]
    public int PetId { get; set; }

    public int? RotinaId { get; set; }

    [Required]
    [MaxLength(120)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    [MaxLength(600)]
    public string Mensagem { get; set; } = string.Empty;

    public DateTime DataAlerta { get; set; }

    public AlertaStatus Status { get; set; } = AlertaStatus.Pendente;

    public Pet Pet { get; set; } = null!;
    public Rotina? Rotina { get; set; }
}

public enum AlertaStatus
{
    Pendente = 1,
    Enviado = 2,
    Lido = 3,
    Cancelado = 4
}

