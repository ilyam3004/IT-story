using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Api.Controllers;

public class FollowersController : ApiController
{
    private readonly IFollowingService _followerService;
    
    public FollowersController(IFollowingService followerService)
    {
        _followerService = followerService;
    }
    
    [HttpGet("followers")]
    public async Task<IActionResult> GetFollowers()
    {
        var token = Request.Headers[HeaderNames.Authorization];
        var followersResult = await _followerService.GetFollowers(token);

        return followersResult.Match(
            result => Ok(result),
            errors => Problem(errors));
    }
    
    [HttpGet("followings")]
    public async Task<IActionResult> GetFollowings()
    {
        var token = Request.Headers[HeaderNames.Authorization];
        var followingsResult = await _followerService.GetFollowings(token);

        return followingsResult.Match(
            result => Ok(result),
            errors => Problem(errors));
    }

    [HttpPost("follow/{followingId}")]
    public async Task<IActionResult> Follow(int followingId)
    {
        var token = Request.Headers[HeaderNames.Authorization];
        var followingResult = await _followerService.Follow(token, followingId);

        return followingResult.Match(
            followResult => Ok(followResult),
            errors => Problem(errors));
    }
    
    [HttpDelete("unfollow/{unFollowingId}")]
    public async Task<IActionResult> UnFollow(int unFollowingId)
    {
        var token = Request.Headers[HeaderNames.Authorization];
        var unFollowingResult = await _followerService.UnFollow(token, unFollowingId);

        return unFollowingResult.Match(
            unFollowResult => Ok(unFollowResult),
            errors => Problem(errors));
    }
}