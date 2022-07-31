using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Api.Controllers;

[Route("followers")]
public class FollowersController : ApiController
{
    // private readonly IFollowerService _followerService;
    //
    // public FollowersController(IFollowerService followerService)
    // {
    //     _followerService = followerService;
    // }
    //
    // [HttpGet("getfollowers")]
    // public async Task<List<FollowersResult>> GetFollowers()
    // {
    //     var token = Request.Headers[HeaderNames.Authorization];
    //     var getFollowersResult = _followerService.GetFollowers(token);
    // }
    //
    // [HttpPost("addfollower")]
    // public async Task<FollowersResult> AddFollower()
    // {
    //     
    // }
}