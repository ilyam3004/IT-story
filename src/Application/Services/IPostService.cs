using ErrorOr;
using Application.Models;

namespace Application.Services;

public interface IPostService
{
    Task<ErrorOr<List<PostResponse>>> GetPosts(string token);
    Task<ErrorOr<PostResponse>> AddPost(string text, string token);
    Task<ErrorOr<MessageResponse>> RemovePost(string token, int postId);
    Task<ErrorOr<PostResponse>> EditPost(int postId, string newText, string token);
    Task<ErrorOr<List<PostResponse>>> GetSavedPosts(string token);

    Task<ErrorOr<MessageResponse>> SavePost(string token, int postId);
    Task<ErrorOr<MessageResponse>> UnSavePost(string token, int postId);
    Task<ErrorOr<PostResponse>> LikePost(string token, int postId);
    Task<ErrorOr<PostResponse>> UnLikePost(string token, int postId);
    Task<ErrorOr<List<PostResponse>>> GetLikedPosts(string token);
    Task<ErrorOr<List<UserLikedPost>>> GetPostLikes(int postId);
    Task<ErrorOr<PostResponse>> CommentPost(string token, int postId, string text);
    Task<ErrorOr<PostResponse>> Reply(string token, int userId, int commentId, string text);
    Task<ErrorOr<MessageResponse>> RemoveReply(string token, int replyId);
    Task<ErrorOr<MessageResponse>> RemoveComment(string token, int commentId);
}