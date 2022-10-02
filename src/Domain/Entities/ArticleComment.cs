using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class ArticleComment
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required] 
    public int ArticleId { get; set; }

    [Required] 
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(150)]
    public string Text { get; set; }
    
    [Required]
    public string Date { get; set; }
}