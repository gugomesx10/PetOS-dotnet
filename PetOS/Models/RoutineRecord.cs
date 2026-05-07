namespace PetOS.Models;

public class RoutineRecord
{
    public long Id { get; set; }
    public long PetId { get; set; }
    public string Type { get; set; }
    public string Decription { get; set; }
    public DateTime Date { get; set; }
    public string Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // navageção
    public Pet Pet { get; set; }
    
}