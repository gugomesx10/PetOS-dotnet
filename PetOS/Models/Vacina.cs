using System.ComponentModel.DataAnnotations;

namespace PetOS.Models;

public class Vacina
{
    public int Id { get; set; }

    [Required]
    public int PetId { get; set; }

    [Required]
    [MaxLength(120)]
    public string Nome { get; set; } = string.Empty;

    public DateOnly DataAplicacao { get; set; }

    public DateOnly? ProximaDose { get; set; }

    [MaxLength(400)]
    public string? Observacoes { get; set; }

    public Pet Pet { get; set; } = null!;
}

