using System.ComponentModel.DataAnnotations;

namespace PetOS.Models;

public class Rotina
{
    public int Id { get; set; }

    [Required]
    public int PetId { get; set; }

    [Required]
    [MaxLength(120)]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(400)]
    public string? Descricao { get; set; }

    public TimeOnly Horario { get; set; }

    public RotinaTipo Tipo { get; set; }

    [MaxLength(30)]
    public string Frequencia { get; set; } = "Diaria";

    public bool Ativa { get; set; } = true;

    public Pet Pet { get; set; } = null!;
    public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
}

public enum RotinaTipo
{
    Alimentacao = 1,
    Higiene = 2,
    Passeio = 3,
    Medicacao = 4,
    Outro = 5
}

