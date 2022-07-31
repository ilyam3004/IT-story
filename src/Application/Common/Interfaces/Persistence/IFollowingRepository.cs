using Domain.Entities;

namespace Application.Common.Interfaces.Persistence;

public interface IFollowingRepository
{
    Task<List<Following>> GetFollowers(int userId);
    Task<List<Following>> GetFollowings(int userId);
    Task AddFollowing(Following following);
    Task RemoveFollowing(Following following);
    Task<Following?> GetFollowingById(int followerId, int followingId);
}