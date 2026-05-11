namespace PetOS.Dto.Alert;

public class AlertCreateDto
{
    public long PetId { get; set; }
    public long? VaccineId { get; set; }
    public string Message { get; set; } =  string.Empty;
    public DateTime AlertDate { get; set; }
}