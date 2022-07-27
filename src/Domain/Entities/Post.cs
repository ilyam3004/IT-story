using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Post
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(319)]
    public string Email { get; set; } = null!;

    [Required]
    [MaxLength(500)]
    public string Text { get; set; } = null!;

    [Required] 
    public string Date { get; set; } = null!;
}