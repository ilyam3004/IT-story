using ErrorOr;
using Application.Models;

namespace Application.Services;

public interface IPostService
{
    Task<ErrorOr<List<PostResult>>> GetPosts(string token);
    Task<ErrorOr<PostResult>> AddPost(string text, string token);
    Task<ErrorOr<PostResult>> RemovePost(int postId);
    Task<ErrorOr<PostResult>> EditPost(int postId, string newText);
}