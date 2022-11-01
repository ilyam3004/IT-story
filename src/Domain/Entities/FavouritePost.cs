using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class FavouritePost
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Favourite_id { get; set; }

    [Required]
    public int User_id { get; set; }

    [Required]
    public int Post_id { get; set; }
}