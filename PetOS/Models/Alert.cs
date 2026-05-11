using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetOS.Models;

public class Alert
{
    [Key]
    public long Id { get; set; }
    [Required]
    [ForeignKey("Pet")]
    public long PetId { get; set; }
    [ForeignKey("Vaccine")]
    public long?  VaccineId { get; set; }
    [Required]
    [StringLength(255)]
    public string Message { get; set; } =  string.Empty;
    [Required]
    public DateTime AlertDate { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // navegação
    public Pet Pet { get; set; }
    
    // relação
    public Vaccine?  Vaccine { get; set; }
}