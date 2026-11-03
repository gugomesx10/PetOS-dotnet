using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetOS.Models;

[Table("T_PETOS_USER")]
public class AppUser
{
    [Key]
    public long Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(160)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(512)]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}