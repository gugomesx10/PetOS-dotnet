using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetOS.Models;

[Table("T_PET")]
public class Pet
{
    [Key]
    public long Id { get; set; }
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    [Required]
    [StringLength(50)]
    public string Species { get; set; }
    [Required]
    [StringLength(100)]
    public string Breed { get; set; }
    [Required]
    public DateTime BirthDate { get; set; }
    [Required]
    [StringLength(30)]
    public string Gender { get; set; }
    [Required]
    [Range(0.1, 9999.99)]
    public decimal Weight { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // e como haverá relação com Vaccine e RoutineRecord
    public ICollection<Vaccine> Vaccines { get; set; }
    public ICollection<RoutineRecord> RoutineRecords { get; set; }
    public ICollection<Alert> Alerts { get; set; }
}