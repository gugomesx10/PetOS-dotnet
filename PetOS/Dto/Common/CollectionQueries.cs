using System.ComponentModel.DataAnnotations;

namespace PetOS.Dto.Common;

public abstract class PageQuery
{
    [Range(1, 100000)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    public string SortBy { get; set; } = "id";

    [RegularExpression(
        "(?i)^(asc|desc)$",
        ErrorMessage = "Use asc ou desc.")]
    public string SortDirection { get; set; } = "asc";
}

public sealed class PetQuery : PageQuery
{
    public string? Name { get; set; }
    public string? Species { get; set; }
}

public sealed class VaccineQuery : PageQuery
{
    public long? PetId { get; set; }
    public string? Name { get; set; }
    public string? Manufacturer { get; set; }
}

public sealed class RoutineQuery : PageQuery
{
    public long? PetId { get; set; }
    public string? Type { get; set; }
}

public sealed class AlertQuery : PageQuery
{
    public long? PetId { get; set; }
    public bool? IsRead { get; set; }
}