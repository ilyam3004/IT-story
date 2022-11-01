using Domain.Entities;

namespace Application.Common.Interfaces.Persistence;

public interface IFollowingRepository
{
    Task<List<Subscribing>> GetFollowers(int userId);
    Task<List<Subscribing>> GetFollowings(int userId);
    Task AddFollowing(Subscribing following);
    Task RemoveFollowing(Subscribing following);
    Task<Subscribing?> GetFollowingById(int followerId, int followingId);
}