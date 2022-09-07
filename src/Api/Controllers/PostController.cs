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
            result => Ok(result),
            errors => Problem(errors));
    }

    [HttpPost("addpost")]
    public async Task<IActionResult> AddPost(PostRequest request)
    {
        var token = Request.Headers[HeaderNames.Authorization];
        
        var postAddResult = await _postService.AddPost(
            request.Text, token);
        
        return postAddResult.Match(
            result => Ok(result),
            errors => Problem(errors));
    }

    [HttpDelete("removepost/{postId}")]
    public async Task<IActionResult> RemovePost(int postId)
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var removePostResult = await _postService.RemovePost(token, postId);
        
        return removePostResult.Match(
            result => Ok(result),
            errors => Problem(errors));
    }

    [HttpPut("editpost")]
    public async Task<IActionResult> EditPost(EditedPost post)
    {
        var token = Request.Headers[HeaderNames.Authorization];
        var editPostResult = await _postService.EditPost(post.PostId, post.NewText, token);
        
        return editPostResult.Match(
            result => Ok(result),
            errors => Problem(errors));
    }
    
    [HttpGet("bookmarks")]
    public async Task<IActionResult> SavedPosts()
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var savedPosts = await _postService.GetSavedPosts(token);

        return savedPosts.Match(
            result => Ok(result),
            errors => Problem(errors));
    }
    
    [HttpPost("savepost/{postId}")]
    public async Task<IActionResult> SavePost(int postId)
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var savedPost = await _postService.SavePost(token, postId);

        return savedPost.Match(
            result => Ok(result),
            errors => Problem(errors));
    }
    
    [HttpDelete("unsavepost/{postId}")]
    public async Task<IActionResult> UnSavePost(int postId)
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var unSavedPost = await _postService.UnSavePost(token, postId);

        return unSavedPost.Match(
            result =>Ok(result),
            errors => Problem(errors));
    }

    [HttpPost("likepost/{postId}")]
    public async Task<IActionResult> LikePost(int postId)
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var likedPost = await _postService.LikePost(token, postId);

        return likedPost.Match(
            result => Ok(result),
            errors => Problem(errors));
    }

    [HttpPost("unlikepost/{postId}")]
    public async Task<IActionResult> UnLikePost(int postId)
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var unLikedPost = await _postService.UnLikePost(token, postId);

        return unLikedPost.Match(
            result => Ok(result),
            errors => Problem(errors));
    }
    
    [HttpGet("likedposts")]
    public async Task<IActionResult> LikedPosts()
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var likedPosts = await _postService.GetLikedPosts(token);

        return likedPosts.Match(
            result => Ok(result),
            errors => Problem(errors));
    }
    
    [HttpGet("likes/{postId}")]
    public async Task<IActionResult> LikedPosts(int postId)
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var likes = await _postService.GetPostLikes(postId);

        return likes.Match(
            result => Ok(result),
            errors => Problem(errors));
    }

    [HttpPost("commentpost")]
    public async Task<IActionResult> CommentPost(CommentRequest request)
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var commentResult = await _postService.CommentPost(token, request.PostId, request.Text);
        return commentResult.Match(
            result => Ok(result),
            errors => Problem(errors));
    }

    [HttpDelete("removeComment/{commentId}")]
    public async Task<IActionResult> RemoveComment(int commentId)
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var removeCommentResult = await _postService.RemoveComment(token, commentId);
        return removeCommentResult.Match(
            result => Ok(result),
            errors => Problem(errors));
    }

    [HttpPost("reply")]
    public async Task<IActionResult> ReplyComment(ReplyRequest request)
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var replyResult = await _postService.Reply(
            token,
            request.UserId,
            request.CommentId,
            request.Text);
        return replyResult.Match(
            result => Ok(result),
            errors => Problem(errors));
    }
    [HttpDelete("removereply/{replyId}")]
    public async Task<IActionResult> RemoveReply(int replyId)
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var replyResult = await _postService.RemoveReply(token, replyId);
        return replyResult.Match(
            result => Ok(result),
            errors => Problem(errors));
    }
}