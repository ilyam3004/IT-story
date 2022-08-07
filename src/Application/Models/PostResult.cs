namespace Application.Models;

public record PostResult(
    int Id,
    int UserId,
    string Text,
    string Date,
    int Likes);