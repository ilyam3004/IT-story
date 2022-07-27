using Application.Services;
using Contracts.Posts;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("posts")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet("getposts")]
    public async Task<IActionResult> GetPosts(string email) 
        => Ok(await _postService.GetPosts(email));
    
    [HttpPost("addpost")]
    public async Task<IActionResult> AddPost(PostRequest request)
    {
        var postAddResult = await _postService.AddPost(
            request.Email, 
            request.Text, 
            request.Date);
        
        var postReponse = new PostResponse(
            postAddResult.Id, 
            postAddResult.Email, 
            postAddResult.Text, 
            postAddResult.Date);
        
        return Ok(postReponse);
    }
}