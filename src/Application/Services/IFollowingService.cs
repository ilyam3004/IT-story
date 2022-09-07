using Application.Models;
using ErrorOr;

namespace Application.Services;

public interface IFollowingService
{
    Task<ErrorOr<List<Follower>>> GetFollowers(string token);
    Task<ErrorOr<List<Follower>>> GetFollowings(string token);
    Task<ErrorOr<FollowingResult>> Follow(string token, int followingId);
    Task<ErrorOr<string>> UnFollow(string token, int unFollowingId); 
}