namespace PetOS.Dto.Routine;

public class RoutineCreateDto
{
    public long PetId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } =  string.Empty;
    public DateTime Date { get; set; }
    public string Notes { get; set; } = string.Empty;
}