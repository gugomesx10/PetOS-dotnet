using System.ComponentModel.DataAnnotations;
using PetOS.Dto.Common;

namespace PetOS.Dto.PetActivity;

/// <summary>
/// Parâmetros de paginação, ordenação e filtros das atividades.
/// </summary>
public class PetActivityQuery : PageQuery
{
    [Range(1, long.MaxValue)]
    public long? PetId { get; set; }

    public string? Type { get; set; }

    public PetActivityQuery()
    {
        SortBy = "createdAtUtc";
        SortDirection = "desc";
    }
}