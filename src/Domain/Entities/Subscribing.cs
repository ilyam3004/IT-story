using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Subscribing
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Subscribing_id { get; set; }

    [Required]
    public int Following_id { get; set; }

    [Required]
    public int Follower_id { get; set; }
}