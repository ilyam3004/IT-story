using Contracts.Posts;

namespace Application.Models;

public record CommentResponse(
    int Id,
    int PostId,
    int UserId,
    string Text,
    DateTime Date,
    List<ReplyResponse> Replies);
