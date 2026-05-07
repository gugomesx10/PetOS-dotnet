namespace PetOS.Models;

public class Pet
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Species { get; set; }
    public string Breed { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; }
    public decimal Weight { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // e como haverá relação com Vaccine e RoutineRecord
    public ICollection<Vaccine> Vaccines { get; set; }
    public ICollection<RoutineRecord> RoutineRecords { get; set; }
    public ICollection<Alert> Alerts { get; set; }
}