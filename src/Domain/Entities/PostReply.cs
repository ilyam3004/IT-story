using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class PostReply
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Reply_id { get; set; }

    [Required]
    public int Comment_id { get; set; }

    [Required]
    public int User_id { get; set; }

    [Required]
    public int Replier_id { get; set; }

    [Required]
    [MaxLength(150)]    
    public string Text { get; set; } = null!;

    [Required]
    public DateTime Date { get; set; }
}