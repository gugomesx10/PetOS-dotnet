namespace PetOS.Dto.Vaccine;

public class VaccineResponseDto
{
    public long Id { get; set; }
    public long PetId { get; set; }
    public string Name { get; set; }
    public string Manufacturer { get; set; }
    public DateTime ApplicationDate { get; set; }
    public DateTime NextDueDate { get; set; }
    public string Dose { get; set; }
    public DateTime CreatedAt { get; set; }
}