using System.ComponentModel.DataAnnotations;

namespace PetOS.Dto.PetActivity;

/// <summary>
/// Dados necessários para cadastrar ou atualizar uma atividade.
/// </summary>
public class PetActivityCreateDto
{
    [Range(1, long.MaxValue)]
    public long PetId { get; set; }

    [Required(ErrorMessage = "O tipo da atividade é obrigatório.")]
    [StringLength(50)]
    public string Type { get; set; } = string.Empty;

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public Dictionary<string, string> Metadata { get; set; } = new();
}