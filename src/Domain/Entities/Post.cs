using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Post
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] 
    public int UserId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Text { get; set; } = null!;

    [Required] 
    public string Date { get; set; } = null!;
}