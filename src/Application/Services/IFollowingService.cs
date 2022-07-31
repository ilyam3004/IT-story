using Application.Models;
using ErrorOr;

namespace Application.Services;

public interface IFollowingService
{
    Task<ErrorOr<FollowingResult>> Follow(string token, int followingId);
    Task<ErrorOr<FollowingResult>> UnFollow(string token, int unFollowingId); 
}