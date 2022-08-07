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
    
    [HttpGet("savedposts")]
    public async Task<IActionResult> SavedPosts()
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var savedPosts = await _postService.GetSavedPosts(token);

        return savedPosts.Match(
            savedPostsResult => Ok(savedPostsResult),
            errors => Problem(errors));
    }
    
    [HttpPost("savepost")]
    public async Task<IActionResult> SavePost(int postId)
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var savedPost = await _postService.SavePost(token, postId);

        return savedPost.Match(
            savedPostResult => Ok(savedPostResult),
            errors => Problem(errors));
    }
    
    [HttpDelete("unsavepost")]
    public async Task<IActionResult> UnSavePost(int postId)
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var unSavedPost = await _postService.UnSavePost(token, postId);

        return unSavedPost.Match(
             unSavePostResult =>Ok(),
            errors => Problem(errors));
    }

    [HttpPost("likepost")]
    public async Task<IActionResult> LikePost(int postId)
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var likedPost = await _postService.LikePost(token, postId);

        return likedPost.Match(
            likeResult => Ok(likeResult),
            errors => Problem(errors));
    }

    [HttpPost("unlikepost")]
    public async Task<IActionResult> UnLikePost(int postId)
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var unLikedPost = await _postService.UnLikePost(token, postId);

        return unLikedPost.Match(
            unLikeResult => Ok(unLikeResult),
            errors => Problem(errors));
    }
    
    [HttpGet("likedposts")]
    public async Task<IActionResult> LikedPosts()
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var likedPosts = await _postService.GetLikedPosts(token);

        return likedPosts.Match(
            likedPostsResult => Ok(likedPostsResult),
            errors => Problem(errors));
    }
    
    [HttpGet("likes")]
    public async Task<IActionResult> LikedPosts(int postId)
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var likes = await _postService.GetPostLikes(postId);

        return likes.Match(
            postLikes => Ok(postLikes),
            errors => Problem(errors));
    }
}