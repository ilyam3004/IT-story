using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Persistence;
using Application.Models;
using Application.Services;
using Domain.Common.Errors;
using Domain.Entities;
using ErrorOr;

namespace Infrastructure.Services;

public class FollowingService : IFollowingService
{
    private readonly IFollowingRepository _followingRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;

    public FollowingService(IFollowingRepository followingRepository, IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository)
    {
        _followingRepository = followingRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<FollowingResult>> Follow(string token, int followingId)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        int followerId = _jwtTokenGenerator.ReadToken(token);
        var userToFollow = await _userRepository.GetUserById(followingId);

        if (userToFollow is null || userToFollow.Id == followerId)
            return Errors.Following.UserToFollowNotFound;

        await _followingRepository.AddFollowing(new Following 
        {
            FollowingId = followingId,
            FollowerId = followerId 
        });

        var following = await _followingRepository.GetFollowingById(followerId, followingId);

        return new FollowingResult(following!.Id, following.FollowingId, following.FollowerId);
    }

    public async Task<ErrorOr<FollowingResult>> UnFollow(string token, int unFollowingId)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        int followerId = _jwtTokenGenerator.ReadToken(token);
        var userToUnFollow = await _userRepository.GetUserById(unFollowingId);

        if (userToUnFollow is null || userToUnFollow.Id == followerId)
            return Errors.Following.UserToUnFollowNotFound;

        var unFollowing = await _followingRepository.GetFollowingById(followerId, unFollowingId);

        if (unFollowing is null)
            return Errors.Following.UserToUnFollowNotFound;

        await _followingRepository.RemoveFollowing(unFollowing);

        return new FollowingResult(unFollowing!.Id, unFollowing.FollowingId, unFollowing.FollowerId);
    }
}