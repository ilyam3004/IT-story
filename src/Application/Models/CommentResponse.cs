using Contracts.Posts;

namespace Application.Models;

public record CommentResponse(
    int Id,
    int PostId,
    int UserId,
    string Text,
    string Date,
    List<ReplyResponse> Replies);
