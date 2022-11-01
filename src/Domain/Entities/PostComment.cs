using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class PostComment
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Comment_id { get; set; }

    [Required]
    public int Post_id { get; set; }

    [Required]
    public int User_id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Text { get; set; } = null!;

    [Required]
    public DateTime Date { get; set; }
}