using Domain.Entities;

namespace Application.Models;

public record PostResult(
    int Id,
    int UserId,
    string Text,
    string Date,
    List<Comment> comments,
    int Likes);