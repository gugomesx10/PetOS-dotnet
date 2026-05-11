using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetOS.Models;

[Table("T_VACCINE")]
public class Vaccine
{
    [Key]
    public long Id { get; set; }
    [Required]
    [ForeignKey("Pet")]
    public long PetId { get; set; }
    [Required]
    [StringLength(100)]
    public string Name { get; set; } =  string.Empty;
    [Required]
    [StringLength(100)]
    public string Manufacturer { get; set; } =   string.Empty;
    [Required]
    public DateTime ApplicationDate { get; set; } 
    [Required]
    public DateTime NextDueDate { get; set; }
    [Required]
    [StringLength(50)]
    public string Dose  { get; set; } = string.Empty ;
    public DateTime CreateAt { get; set; }
    
    // pra navegação, blz
    public Pet Pet { get; set; }
    
    // relação 
    public ICollection<Alert> Alerts { get; set; } =  new List<Alert>();
}                                           