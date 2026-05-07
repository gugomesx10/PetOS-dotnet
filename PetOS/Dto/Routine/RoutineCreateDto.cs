namespace PetOS.Dto.Routine;

public class RoutineCreateDto
{
    public long PetId { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public string Notes { get; set; }
}