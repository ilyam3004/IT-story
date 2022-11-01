using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class ArticleLike
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Like_id { get; set; }

    [Required]
    public int User_id { get; set; }

    [Required]
    public int Article_id { get; set; }
    
    [Required]
    public int Score { get; set; }
}
