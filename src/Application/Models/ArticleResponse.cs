namespace Application.Models;

public record ArticleResponse(
    int Id,
    int UserId,
    string Title,
    string Text,
    DateTime Date,
    List<ArticleCommentResponse> Comments,
    int Likes,
    double AvgScore);