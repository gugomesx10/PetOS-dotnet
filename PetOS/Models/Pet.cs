using System.ComponentModel.DataAnnotations;

namespace PetOS.Models;

public class Pet
{
    public int Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string Especie { get; set; } = string.Empty;

    [MaxLength(80)]
    public string? Raca { get; set; }

    public DateOnly? DataNascimento { get; set; }

    [Required]
    [MaxLength(120)]
    public string ResponsavelNome { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? ResponsavelTelefone { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<Vacina> Vacinas { get; set; } = new List<Vacina>();
    public ICollection<Rotina> Rotinas { get; set; } = new List<Rotina>();
    public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
}

