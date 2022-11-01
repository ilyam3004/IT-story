namespace Application.Models;
public record ArticleCommentResponse(
    int Id,
    int ArticleId,
    int UserId,
    string Text,
    DateTime Date,
    List<ArticleReplyResponse> Replies,
    bool IsAuthor);