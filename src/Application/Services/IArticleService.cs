using ErrorOr;
using Application.Models;

namespace Application.Services;

public interface IArticleService
{
    Task<ErrorOr<List<ArticleResponse>>> GetUserArticles(string token);
    Task<ErrorOr<ArticleResponse>> AddArticle(string title, string text, string token);
    Task<ErrorOr<MessageResponse>> RemoveArticle(string token, int articleId);
    Task<ErrorOr<ArticleResponse>> EditArticleText(int articleId, string newText, string token);
    Task<ErrorOr<ArticleResponse>> EditArticleTitle(int articleId, string newText, string token);
    Task<ErrorOr<ArticleResponse>> LikeArticle(string token, int articleId, int score);
    Task<ErrorOr<ArticleResponse>> UnLikeArticle(string token, int articleId);
    Task<ErrorOr<List<ArticleResponse>>> GetLikedArticles(string token);
    Task<ErrorOr<List<UserLikedPost>>> GetArticleLikes(int articleId);
    Task<ErrorOr<ArticleResponse>> CommentArticle(string token, int articleId, string text);
    Task<ErrorOr<ArticleResponse>> Reply(string token, int userId, int commentId, string text);
    Task<ErrorOr<MessageResponse>> RemoveReply(string token, int replyId);
    Task<ErrorOr<MessageResponse>> RemoveComment(string token, int commentId);
}