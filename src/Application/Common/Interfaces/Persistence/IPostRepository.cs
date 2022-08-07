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
    Task<SavedPost?> GetSavedPostById(int id);
    Task<Post> LikePost(Post post);
    Task<Post> UnLikePost(Post post);
    Task<List<Like>> LikedPosts(int userId);
    Task<Like> GetLikeByPostId(int userId, int postId);
    Task<List<Like>> GetPostLikes(int postId);
}