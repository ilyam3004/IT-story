using Application.Common.Interfaces.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Persistence;

public class PostRepository : IPostRepository
{
    private readonly AppDbContext _db;

    public PostRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Post>> GetPostsByUserId(int userId)
        => await _db.Posts.Where(post => post.UserId == userId).ToListAsync();

    public async Task<Post?> GetPostById(int postId)
        => await _db.Posts.FirstOrDefaultAsync(post => post.Id == postId);
    
    public async Task AddPost(Post post)
    {
        await _db.Posts.AddAsync(post);
        await _db.SaveChangesAsync();
    }

    public async Task RemovePost(int postId)
    {
        var post = await _db.Posts.FirstOrDefaultAsync(post => post.Id == postId);
        _db.Posts.Remove(post!);
        await _db.SaveChangesAsync();
    }

    public async Task<Post> EditPost(int postId, string newText)
    {
        var post = await _db.Posts.FirstOrDefaultAsync(post => post.Id == postId);
        post!.Text = newText;
        await _db.SaveChangesAsync();
        return post;
    }
}