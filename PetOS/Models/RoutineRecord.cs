using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetOS.Models;

[Table("T_ROUTINE_RECORD")]
public class RoutineRecord
{
    [Key]
    public long Id { get; set; }
    [Required]
    [ForeignKey("Pet")]
    public long PetId { get; set; }
    [Required]
    [StringLength(50)]
    public string Type { get; set; } =  string.Empty;
    [Required]
    [StringLength(255)]
    public string Description { get; set; } =  string.Empty;
    [Required]
    public DateTime Date { get; set; }
    [StringLength(500)]
    public string Notes { get; set; } =  string.Empty;
    public DateTime CreatedAt { get; set; }
    
    // navageção
    public Pet? Pet { get; set; }
    
}