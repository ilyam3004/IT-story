using Domain.Entities;

namespace Application.Common.Interfaces.Persistence;

public interface IPostRepository
{
    Task<List<Post>> GetPostsByUserId(int userId);
    Task<Post?> GetPostById(int postId);
    Task<Post> AddPost(Post post);
    Task RemovePost(Post post);
    Task<Post> EditPost(Post post, string newText);
    Task<List<SavedPost>> GetSavedPosts(int id);
    Task SavePost(SavedPost post);
    Task UnSavePost(SavedPost post);
    Task<SavedPost?> GetSavedPostById(int id);
    Task<Post> LikePost(Post post, int userId);
    Task<Post> UnLikePost(Post post, int userId);
    Task<List<Like>> LikedPosts(int userId);
    Task<Like> GetLikeByPostId(int userId, int postId);
    Task<List<Like>> GetPostLikes(int postId);
    Task CreateComment(Comment comment);
    Task RemoveComment(Comment comment);
    Task<List<Comment>> GetComments(int postId);
    Task<Comment?> GetCommentById(int commentId);
    Task ReplyComment(Reply reply);
    Task RemoveReply(Reply reply);
    Task<List<Reply>> GetReplies(int commentId);
    Task<Reply?> GetReplyById(int replyId);
}