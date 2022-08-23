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
            .Where(post => post.UserId == userId)
            .ToListAsync();

    public async Task<Post?> GetPostById(int postId)
        => await _db.Posts.FirstOrDefaultAsync(post => post.Id == postId);
    
    public async Task<Post> AddPost(Post post)
    {
        await _db.Posts.AddAsync(post);
        await _db.SaveChangesAsync();
        return (await _db.Posts.FirstOrDefaultAsync(p => p.UserId == post.UserId & p.Date == post.Date))!;
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

    public async Task<List<SavedPost>> GetSavedPosts(int id)
        => await _db.SavedPosts
            .Where(post => post.UserId == id)
            .ToListAsync();

    public async Task<SavedPost?> GetSavedPostById(int id)
        => await _db.SavedPosts.FirstOrDefaultAsync(post => post.PostId == id);

    public async Task SavePost(SavedPost post)
    {
        await _db.SavedPosts.AddAsync(post);
        await _db.SaveChangesAsync();
    }
    
    public async Task UnSavePost(SavedPost post)
    {
        _db.SavedPosts.Remove(post!);
        await _db.SaveChangesAsync();
    }

    public async Task<Post> LikePost(Post post)
    {
        post.Likes++;
        await _db.Likes.AddAsync(new Like
        {
            UserId = post.UserId,
            PostId = post.Id
        });
        await _db.SaveChangesAsync();
        return post;
    }

    public async Task<Post> UnLikePost(Post post)
    {
        post.Likes--;
        var likeToRemove = await GetLikeByPostId(post.UserId, post.Id);
        _db.Likes.Remove(likeToRemove);
        await _db.SaveChangesAsync();
        return post;
    }

    public async Task<List<Like>> LikedPosts(int userId)
        => await _db.Likes
            .Where(like => like.UserId == userId)
            .ToListAsync();

    public async Task<Like> GetLikeByPostId(int userId, int postId)
        => (await _db.Likes
            .FirstOrDefaultAsync(like => like.PostId == postId && like.UserId == userId))!;

    public async Task<List<Like>> GetPostLikes(int postId)
        => await _db.Likes.Where(like => like.PostId == postId).ToListAsync();

    public async Task CreateComment(Comment comment)
    {
        await _db.Comments.AddAsync(comment);
        await _db.SaveChangesAsync();
    }

    public async Task<List<Comment>> GetComments(int postId)
        => await _db.Comments
            .Where(comment => comment.PostId == postId)
            .ToListAsync();

    public async Task RemoveComment(Comment comment)
    {
        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();
    }

    public async Task<Comment?> GetCommentById(int commentId)
        => await _db.Comments
            .FirstOrDefaultAsync(comment => comment.Id == commentId );

    public async Task ReplyComment(Reply reply)
    {
        await _db.Replies.AddAsync(reply);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveReply(Reply reply)
    {
        _db.Replies.Remove(reply);
        await _db.SaveChangesAsync();
    }
    public async Task<List<Reply>> GetReplies(int commentId)
        => await _db.Replies
            .Where(reply => reply.CommentId == commentId)
            .ToListAsync();
    
    public async Task<Reply?> GetReplyById(int replyId)
        => await _db.Replies.FirstOrDefaultAsync(r => r.Id == replyId);
    
}