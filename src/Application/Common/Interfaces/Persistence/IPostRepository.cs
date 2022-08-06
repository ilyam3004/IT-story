using Domain.Entities;

namespace Application.Common.Interfaces.Persistence;

public interface IPostRepository
{
    Task<List<Post>> GetPostsByUserId(int userId);
    Task<Post?> GetPostById(int postId);
    Task AddPost(Post post);
    Task RemovePost(Post post);
    Task<Post> EditPost(Post post, string newText);
    Task<List<SavedPost>> GetSavedPosts(int id);
    Task SavePost(SavedPost post);
    Task UnSavePost(SavedPost post);
}