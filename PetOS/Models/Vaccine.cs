namespace PetOS.Models;

public class Vaccine
{
    public long Id { get; set; }
    public long PetId { get; set; }
    public string Name { get; set; }
    public string Manufacturer { get; set; }
    public DateTime ApplicationDate { get; set; } 
    public DateTime NextDueDate { get; set; }
    public string Dose  { get; set; }
    public DateTime CreateAt { get; set; }
    
    // pra navegação, blz
    public Pet Pet { get; set; }
    
    // relação 
    public ICollection<Alert> Alerts { get; set; }
}                                           