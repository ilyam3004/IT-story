using Domain.Entities;

namespace Contracts.Posts;

public record ReplyResponse(
    int Id,
    int CommentId,
    User User,
    User Replier,
    string Text,
    string Data);