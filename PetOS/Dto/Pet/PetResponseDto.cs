namespace PetOS.Dto.Pet;

public class PetResponseDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Species { get; set; }
    public string Breed { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; }
    public decimal weight { get; set; }
    public DateTime CreatedAt { get; set; }
}