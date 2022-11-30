using Domain.Entities;

namespace Application.Models;

public record PostReplyResponse(
    int Id,
    int CommentId,
    User User,
    User Replier,
    string Text,
    DateTime Date);