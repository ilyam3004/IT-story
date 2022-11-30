namespace Application.Models;

public record PostCommentResponse(
    int Id,
    int PostId,
    int UserId,
    string Text,
    DateTime Date,
    List<PostReplyResponse> Replies);
