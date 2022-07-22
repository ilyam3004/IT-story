using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;
    
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }

    [Required]
    [MaxLength(10)]
    public string username { get; set; } = null!;

    [Required]
    [MaxLength(319)]
    public string email { get; set; } = null!;

    [Required]
    [MaxLength(60)]
    public string password { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string firstName { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string lastName { get; set; } = null!;
    
    [Required]
    [MaxLength(10)]
    public string status { get; set; } = null!;
}
