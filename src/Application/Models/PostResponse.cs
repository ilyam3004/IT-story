namespace Application.Models;

public record PostResponse(
    int PostId,
    int UserId,
    string Text,
    DateTime Date,
    List<PostCommentResponse> Comments,
    int Likes);