using Application.Common.Interfaces.Persistence;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Persistence;

public class PostRepository : IPostRepository
{
    private readonly PostDbContext _db;

    public PostRepository(PostDbContext db)
    {
        _db = db;
    }

    public async Task<List<Post>> GetPostsByEmail(string email)
        => await _db.Posts.Where(post => post.Email == email).ToListAsync();


    public async Task AddPost(Post post)
        => await _db.Posts.AddAsync(post);

    public Task RemovePost(int id)
    {
        throw new NotImplementedException();
    }
}