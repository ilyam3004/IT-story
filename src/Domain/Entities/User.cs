using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int User_id { get; set; }

    [Required]
    [MaxLength(10)]
    public string Username { get; set; } = null!;

    [Required]
    [MaxLength(319)]
    public string Email { get; set; } = null!;

    [Required]
    [MaxLength(60)]
    public string Password { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = null!;
}
