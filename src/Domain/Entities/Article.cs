﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Article
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Article_id { get; set; }

    [Required]
    public int User_id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = null!;

    [Required]
    [MaxLength(5000)]
    public string Text { get; set; } = null!;

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public int Likes_count { get; set; }

    [Required]
    public double Avg_score { get; set; }
}