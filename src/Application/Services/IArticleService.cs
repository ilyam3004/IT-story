using ErrorOr;
using Application.Models;

namespace Application.Services;

public interface IArticleService
{
    Task<ErrorOr<List<ArticleResult>>> GetArticles(string token);
    Task<ErrorOr<ArticleResult>> AddArticle(string title, string text, string token);
    Task<ErrorOr<Message>> RemoveArticle(string token, int articleId);
    Task<ErrorOr<ArticleResult>> EditArticleText(int articleId, string newText, string token);
    Task<ErrorOr<ArticleResult>> EditArticleTitle(int articleId, string newText, string token);
    Task<ErrorOr<ArticleResult>> LikeArticle(string token, int articleId, int score);
    Task<ErrorOr<ArticleResult>> UnLikeArticle(string token, int articleId);
    Task<ErrorOr<List<ArticleResult>>> GetLikedArticles(string token);
    Task<ErrorOr<List<UserLikedPost>>> GetArticleLikes(int articleId);
    Task<ErrorOr<ArticleResult>> CommentArticle(string token, int articleId, string text);
    Task<ErrorOr<ArticleResult>> Reply(string token, int userId, int commentId, string text);
    Task<ErrorOr<Message>> RemoveReply(string token, int replyId);
    Task<ErrorOr<Message>> RemoveComment(string token, int commentId);
}