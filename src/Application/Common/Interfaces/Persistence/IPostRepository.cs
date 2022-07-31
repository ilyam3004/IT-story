using Domain.Entities;

namespace Application.Common.Interfaces.Persistence;

public interface IPostRepository
{
    Task<List<Post>> GetPostsByUserId(int userId);
    Task<Post?> GetPostById(int postId);
    Task AddPost(Post post);
    Task RemovePost(int postId);
    Task<Post> EditPost(int postId, string newText);
}