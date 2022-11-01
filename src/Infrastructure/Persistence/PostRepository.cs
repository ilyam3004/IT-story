using Application.Common.Interfaces.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class PostRepository : IPostRepository
{
    private readonly AppDbContext _db;

    public PostRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Post>> GetPostsByUserId(int userId)
        => await _db.Posts
            .Where(post => post.User_id == userId)
            .ToListAsync();

    public async Task<Post?> GetPostById(int postId)
        => await _db.Posts.FirstOrDefaultAsync(post => post.Post_id == postId);

    public async Task<Post> AddPost(Post post)
    {
        await _db.Posts.AddAsync(post);
        await _db.SaveChangesAsync();
        return (await _db.Posts.FirstOrDefaultAsync(p =>
            p.User_id == post.User_id &
            p.Date == post.Date))!;
    }

    public async Task RemovePost(Post post)
    {
        _db.Posts.Remove(post!);
        await _db.SaveChangesAsync();
    }

    public async Task<Post> EditPost(Post post, string newText)
    {
        post!.Text = newText;
        await _db.SaveChangesAsync();
        return post;
    }

    public async Task<List<FavouritePost>> GetSavedPosts(int id)
        => await _db.SavedPosts.Where(post => post.User_id == id)
            .ToListAsync();

    public async Task<FavouritePost?> GetSavedPostById(int id)
        => await _db.SavedPosts.FirstOrDefaultAsync(post => post.Post_id == id);

    public async Task SavePost(FavouritePost post)
    {
        await _db.SavedPosts.AddAsync(post);
        await _db.SaveChangesAsync();
    }

    public async Task UnSavePost(FavouritePost post)
    {
        _db.SavedPosts.Remove(post!);
        await _db.SaveChangesAsync();
    }

    public async Task<Post> LikePost(Post post, int userId)
    {
        post.Likes_count++;
        await _db.Likes.AddAsync(new PostLike
        {
            User_id = userId,
            Post_id = post.Post_id
        });
        await _db.SaveChangesAsync();
        return post;
    }

    public async Task<Post> UnLikePost(Post post, int userId)
    {
        post.Likes_count--;
        var likeToRemove = await GetLikeByPostId(userId, post.Post_id);
        _db.Likes.Remove(likeToRemove);
        await _db.SaveChangesAsync();
        return post;
    }

    public async Task<List<PostLike>> LikedPosts(int userId)
        => await _db.Likes
            .Where(like => like.User_id == userId)
            .ToListAsync();

    public async Task<PostLike> GetLikeByPostId(int userId, int postId)
        => (await _db.Likes
            .FirstOrDefaultAsync(like => like.Post_id == postId &&
                                         like.User_id == userId))!;

    public async Task<List<PostLike>> GetPostLikes(int postId)
        => await _db.Likes
            .Where(like => like.Post_id == postId)
            .ToListAsync();

    public async Task CreateComment(PostComment comment)
    {
        await _db.Comments.AddAsync(comment);
        await _db.SaveChangesAsync();
    }

    public async Task<List<PostComment>> GetComments(int postId)
        => await _db.Comments
            .Where(comment => comment.Post_id == postId)
            .ToListAsync();

    public async Task RemoveComment(PostComment comment)
    {
        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();
    }

    public async Task<PostComment?> GetCommentById(int commentId)
        => await _db.Comments
            .FirstOrDefaultAsync(comment => comment.Comment_id == commentId);

    public async Task ReplyComment(PostReply reply)
    {
        await _db.Replies.AddAsync(reply);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveReply(PostReply reply)
    {
        _db.Replies.Remove(reply);
        await _db.SaveChangesAsync();
    }
    public async Task<List<PostReply>> GetReplies(int commentId)
        => await _db.Replies
            .Where(reply => reply.Comment_id == commentId)
            .ToListAsync();

    public async Task<PostReply?> GetReplyById(int replyId)
        => await _db.Replies.FirstOrDefaultAsync(r => r.Reply_id == replyId);

}