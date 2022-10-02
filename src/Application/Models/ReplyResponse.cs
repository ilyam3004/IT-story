using Domain.Entities;

namespace Application.Models;

public record ReplyResponse(
    int Id,
    int CommentId,
    User User,
    User Replier,
    string Text,
    string Data);