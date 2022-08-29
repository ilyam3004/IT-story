using ErrorOr;
using Application.Models;

namespace Application.Services;

public interface IPostService
{
    Task<ErrorOr<List<PostResult>>> GetPosts(string token);
    Task<ErrorOr<PostResult>> AddPost(string text, string token);
    Task<ErrorOr<string>> RemovePost(string token, int postId);
    Task<ErrorOr<PostResult>> EditPost(int postId, string newText, string token);
    Task<ErrorOr<List<PostResult>>> GetSavedPosts(string token);

    Task<ErrorOr<PostResult>> SavePost(string token, int postId);
    Task<ErrorOr<string>> UnSavePost(string token, int postId);
    Task<ErrorOr<PostResult>> LikePost(string token, int postId);
    Task<ErrorOr<PostResult>> UnLikePost(string token, int postId);
    Task<ErrorOr<List<PostResult>>> GetLikedPosts(string token);
    Task<ErrorOr<List<UserLikedPost>>> GetPostLikes(int postId);
    Task<ErrorOr<PostResult>> CommentPost(string token, int postId, string text);
    Task<ErrorOr<PostResult>> Reply(string token, int userId, int commentId, string text);
    Task<ErrorOr<string>> RemoveReply(string token, int replyId);
    Task<ErrorOr<string>> RemoveComment(string token, int commentId);
}