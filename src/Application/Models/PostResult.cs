namespace Application.Models;

public record PostResult(
    int Id,
    int UserId,
    string Text,
    DateTime Date,
    List<CommentResponse> Comments,
    int Likes);