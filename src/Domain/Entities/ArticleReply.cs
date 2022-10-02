using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class ArticleReply
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public int ArticleCommentId { get; set; }
    
    [Required]
    public int UserId { get; set; }

    [Required] 
    public int ReplierId { get; set; }
    
    [Required] 
    public string Text { get; set; }

    [Required] 
    public string Data { get; set; }
}