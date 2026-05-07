namespace PetOS.Dto.Alert;

public class AlertResponseDto
{
    public long Id { get; set; }
    public long PetId { get; set; }
    public long? VaccineId { get; set; }
    public string Message { get; set; }
    public DateTime AlertDate { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}