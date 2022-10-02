namespace Application.Models;
public record ArticleCommentResponse(
    int Id,
    int ArticleId,
    int UserId,
    string Text,
    string Date,
    List<ArticleReplyResponse> Replies);