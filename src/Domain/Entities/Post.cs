using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Post
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Post_id { get; set; }

    [Required]
    public int User_id { get; set; }

    [Required]
    [MaxLength(500)]
    public string Text { get; set; } = null!;

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public int Likes_count { get; set; }
}