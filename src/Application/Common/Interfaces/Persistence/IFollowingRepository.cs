using Domain.Entities;

namespace Application.Common.Interfaces.Persistence;

public interface IFollowingRepository
{
    Task AddFollowing(Following following);
    Task RemoveFollowing(Following following);
    Task<Following?> GetFollowingById(int followerId, int followingId);
}