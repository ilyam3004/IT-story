using Domain.Entities;

namespace Application.Services;

public interface IPostService
{
    Task<List<Post>> GetPosts(string email);
    Task<Post> AddPost(string email, string text, string date);
    Task RemovePost(int id);
}