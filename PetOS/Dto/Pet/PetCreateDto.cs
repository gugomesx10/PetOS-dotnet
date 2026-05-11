namespace PetOS.Dto.Pet;

public class PetCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } =  string.Empty;
    public string Breed { get; set; } =   string.Empty;
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; } =   string.Empty;
    public decimal weight { get; set; } 
}