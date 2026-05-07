namespace PetOS.Dto.Routine;

public class RoutineResponseDto
{
    public long Id { get; set; }
    public long PetId { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public string Notes { get; set; }
    public DateTime Created { get; set; }
}