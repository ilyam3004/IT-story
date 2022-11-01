using Domain.Entities;

namespace Application.Common.Interfaces.Persistence;

public interface IArticleRepository
{
    Task<List<Article>> GetArticlesByUserId(int userId);
    Task<Article?> GetArticleById(int postId);
    Task<Article> AddArticle(Article article);
    Task RemoveArticle(Article article);
    Task<Article> EditArticleText(Article article, string newText);
    Task<Article> EditArticleTitle(Article article, string newTitle);
    Task<Article> LikeArticle(Article article, int userId, int score, double avgScore);
    Task<Article> UnLikeArticle(Article article, int userId, double avgScore);
    Task<List<ArticleLike>> LikedArticles(int userId);
    Task<ArticleLike> GetLikeByArticleId(int userId, int articleId);
    Task<List<ArticleLike>> GetArticleLikes(int articleId);
    Task CreateComment(ArticleComment comment);
    Task RemoveComment(ArticleComment comment);
    Task<List<ArticleComment>> GetComments(int articleId);
    Task<ArticleComment?> GetCommentById(int commentId);
    Task ReplyComment(ArticleReply reply);
    Task RemoveReply(ArticleReply reply);
    Task<List<ArticleReply>> GetReplies(int commentId);
    Task<ArticleReply?> GetReplyById(int replyId);
}