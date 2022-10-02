using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Article
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] 
    public int UserId { get; set; }

    [Required]
    [MaxLength(5000)]
    public string Text { get; set; } = null!;

    [Required] 
    public string Date { get; set; } = null!;
    
    public int Likes { get; set; }
}