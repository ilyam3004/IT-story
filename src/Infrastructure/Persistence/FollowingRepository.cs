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
    
    //TODO make get followings and followers

    public async Task AddFollowing(Following following)
    {
        await _db.Followings.AddAsync(following);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveFollowing(Following following)
    {
        _db.Followings.Remove(following);
        await _db.SaveChangesAsync();
    }

    public async Task<Following?> GetFollowingById(int followerId, int followingId)
        => await _db.Followings.FirstOrDefaultAsync(
            following => following.FollowingId == followingId && 
                         following.FollowerId == followerId);
}