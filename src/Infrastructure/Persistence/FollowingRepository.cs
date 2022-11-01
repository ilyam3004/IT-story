using Application.Common.Interfaces.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class FollowingRepository : IFollowingRepository
{
    private readonly AppDbContext _db;

    public FollowingRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Subscribing>> GetFollowers(int userId)
        => await _db.Followings
            .Where(following => following.Following_id == userId)
            .ToListAsync();

    public async Task<List<Subscribing>> GetFollowings(int userId)
        => await _db.Followings
            .Where(following => following.Follower_id == userId)
            .ToListAsync();

    public async Task AddFollowing(Subscribing following)
    {
        await _db.Followings.AddAsync(following);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveFollowing(Subscribing following)
    {
        _db.Followings.Remove(following);
        await _db.SaveChangesAsync();
    }

    public async Task<Subscribing?> GetFollowingById(int followerId, int followingId)
        => await _db.Followings.FirstOrDefaultAsync(
            following => following.Following_id == followingId &&
                         following.Follower_id == followerId);
}