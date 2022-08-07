using ErrorOr;
using Application.Models;

namespace Application.Services;

public interface IPostService
{
    Task<ErrorOr<List<PostResult>>> GetPosts(string token);
    Task<ErrorOr<PostResult>> AddPost(string text, string token);
    Task<ErrorOr<PostResult>> RemovePost(int postId);
    Task<ErrorOr<PostResult>> EditPost(int postId, string newText);
    Task<ErrorOr<List<PostResult>>> GetSavedPosts(string token);

    Task<ErrorOr<PostResult>> SavePost(string token, int postId);
    Task<ErrorOr<PostResult>> UnSavePost(string token, int postId);
    Task<ErrorOr<PostResult>> LikePost(string token, int postId);
    Task<ErrorOr<PostResult>> UnLikePost(string token, int postId);
    Task<ErrorOr<List<PostResult>>> GetLikedPosts(string token);
    Task<ErrorOr<List<UserLikedPost>>> GetPostLikes(int postId);
}