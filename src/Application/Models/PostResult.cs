namespace Application.Models;

public record PostResult(
    int Id,
    int UserId,
    string Text,
    string Date,
    List<CommentResult> Comments,
    int Likes);