using Domain.Entities;

namespace Application.Common.Interfaces.Persistence;

public interface IPostRepository
{
    Task<List<Post>> GetPostsByUserId(int userId);
    Task<Post?> GetPostById(int postId);
    Task<Post> AddPost(Post post);
    Task RemovePost(Post post);
    Task<Post> EditPost(Post post, string newText);
    Task<List<FavouritePost>> GetSavedPosts(int id);
    Task SavePost(FavouritePost post);
    Task UnSavePost(FavouritePost post);
    Task<FavouritePost?> GetSavedPostById(int id);
    Task<Post> LikePost(Post post, int userId);
    Task<Post> UnLikePost(Post post, int userId);
    Task<List<PostLike>> LikedPosts(int userId);
    Task<PostLike> GetLikeByPostId(int userId, int postId);
    Task<List<PostLike>> GetPostLikes(int postId);
    Task CreateComment(PostComment comment);
    Task RemoveComment(PostComment comment);
    Task<List<PostComment>> GetComments(int postId);
    Task<PostComment?> GetCommentById(int commentId);
    Task ReplyComment(PostReply reply);
    Task RemoveReply(PostReply reply);
    Task<List<PostReply>> GetReplies(int commentId);
    Task<PostReply?> GetReplyById(int replyId);
}