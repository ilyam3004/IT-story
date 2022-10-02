namespace Contracts.Articles;

public record ArticleCommentRequest(
    int ArticleId,
    string Text);