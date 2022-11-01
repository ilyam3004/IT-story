using Domain.Entities;

namespace Application.Models;

public record ArticleReplyResponse(
    int Id,
    int CommentId,
    User User,
    User Replier,
    string Text,
    DateTime Date,
    bool isAuthor);
