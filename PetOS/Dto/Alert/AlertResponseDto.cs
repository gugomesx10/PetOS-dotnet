namespace PetOS.Dto.Alert;

public class AlertResponseDto
{
    public long Id { get; set; }
    public long PetId { get; set; }
    public long? VaccineId { get; set; }
    public string Message { get; set; } =  string.Empty;
    public DateTime AlertDate { get; set; }
    public int IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}