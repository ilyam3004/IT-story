namespace Application.Models;

public record ArticleResult(
    int Id,
    int UserId,
    string Text,
    string Date,
    List<ArticleCommentResponse> Comments,
    int Likes);