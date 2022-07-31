using Application.Models;
using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Contracts.Posts;
using Microsoft.Net.Http.Headers;

namespace Api.Controllers;

[Route("posts")]
public class PostController : ApiController
{
    private readonly IPostService _postService;
    
    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPosts()
    {
        var token = Request.Headers[HeaderNames.Authorization];
        var posts = await _postService.GetPosts(token);
        return posts.Match(
            postsResult => Ok(postsResult),
            errors => Problem(errors));
    }

    [HttpPost("addpost")]
    public async Task<IActionResult> AddPost(PostRequest request)
    {
        var token = Request.Headers[HeaderNames.Authorization];
        
        var postAddResult = await _postService.AddPost(
            request.Text, token);
        
        return postAddResult.Match(
            postAddingResult => Ok(postAddingResult),
            errors => Problem(errors));
    }

    [HttpDelete("removepost")]
    public async Task<IActionResult> RemovePost(int postId)
    {
        var removePostResult = await _postService.RemovePost(postId);
        
        return removePostResult.Match(
            removingPostResult => Ok(removingPostResult),
            errors => Problem(errors));
    }

    [HttpPut("editpost")]
    public async Task<IActionResult> EditPost(EditedPost post)
    {
        var editPostResult = await _postService.EditPost(post.PostId, post.NewText);
        
        return editPostResult.Match(
            editingPostResult => Ok(editingPostResult),
            errors => Problem(errors));
    }
}