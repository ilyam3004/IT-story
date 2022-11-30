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

    public async Task<ErrorOr<List<Follower>>> GetFollowers(string token)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var userId = _jwtTokenGenerator.ReadToken(token);
        List<Subscribing> dbFollowers = await _followingRepository.GetFollowers(userId);

        if (dbFollowers.Count == 0)
            return Errors.Following.FollowersNotFound;

        List<Follower> followers = new();
        foreach (var item in dbFollowers)
        {
            var follower = await _userRepository.GetUserById(item.Follower_id);
            if (follower is null)
                return Errors.Following.FollowersNotFound;
            followers.Add(new Follower(
                follower.User_id,
                follower.Username,
                follower.FirstName,
                follower.LastName));
        }
        return followers;
    }

    public async Task<ErrorOr<List<Follower>>> GetFollowings(string token)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var userId = _jwtTokenGenerator.ReadToken(token);
        List<Subscribing> dbFollowings = await _followingRepository.GetFollowings(userId);

        if (dbFollowings.Count == 0)
            return Errors.Following.FollowingsNotFound;

        List<Follower> followings = new();
        foreach (var item in dbFollowings)
        {
            var follower = await _userRepository.GetUserById(item.Following_id);
            if (follower is null)
                return Errors.Following.FollowersNotFound;
            followings.Add(new Follower(
                follower.User_id,
                follower.Username,
                follower.FirstName,
                follower.LastName));
        }
        return followings;
    }

    public async Task<ErrorOr<FollowingResult>> Follow(string token, int followingId)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        int followerId = _jwtTokenGenerator.ReadToken(token);
        var userToFollow = await _userRepository.GetUserById(followingId);

        if (userToFollow is null || userToFollow.User_id == followerId)
            return Errors.Following.UserToFollowNotFound;

        await _followingRepository.AddFollowing(new Subscribing
        {
            Following_id = followingId,
            Follower_id = followerId
        });

        var subscribing = await _followingRepository
            .GetFollowingById(followerId, followingId);

        return new FollowingResult(subscribing!.Subscribing_id, subscribing.Following_id, subscribing.Follower_id);
    }

    public async Task<ErrorOr<MessageResponse>> UnFollow(string token, int unFollowingId)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        int followerId = _jwtTokenGenerator.ReadToken(token);
        var userToUnFollow = await _userRepository.GetUserById(unFollowingId);

        if (userToUnFollow is null || userToUnFollow.User_id == followerId)
            return Errors.Following.UserToUnFollowNotFound;

        var unFollowing = await _followingRepository.GetFollowingById(followerId, unFollowingId);

        if (unFollowing is null)
            return Errors.Following.UserToUnFollowNotFound;

        await _followingRepository.RemoveFollowing(unFollowing);

        return new MessageResponse(Correct.Followings.Unfollow
                           + $" {userToUnFollow.Username}");
    }
}