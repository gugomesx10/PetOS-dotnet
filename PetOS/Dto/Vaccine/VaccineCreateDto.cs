namespace PetOS.Dto.Vaccine;

public class VaccineCreateDto
{
    public long PetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public DateTime ApplicationDate { get; set; }
    public DateTime NextDueDate { get; set; }
    public string Dose { get; set; } = string.Empty;
}