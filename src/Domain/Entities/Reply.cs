using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Reply
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public int CommentId { get; set; }
    
    [Required]
    public int UserId { get; set; }

    [Required] 
    public int ReplierId { get; set; }
    
    [Required] 
    public string Text { get; set; }

    [Required] 
    public string Data { get; set; }
}