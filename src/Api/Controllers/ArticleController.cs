using Application.Models;
using Application.Services;
using Contracts.Articles;
using Contracts.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Api.Controllers;

[Route("articles")]
public class ArticleController : ApiController
{
    private readonly IArticleService _articleService;
    
    public ArticleController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetArticles()
    {
        var token = Request.Headers[HeaderNames.Authorization];
        var articles = await _articleService.GetArticles(token);
        return articles.Match(
            result => Ok(result),
            errors => Problem(errors));
    }

    [HttpPost("addarticle")]
    public async Task<IActionResult> AddArticle(ArticleRequest request)
    {
        var token = Request.Headers[HeaderNames.Authorization];
        
        var articleAddResult = await _articleService.AddArticle(
            request.Title,
            request.Text, 
            token);
        
        return articleAddResult.Match(
            result => Ok(result),
            errors => Problem(errors));
    }

    [HttpDelete("removearticle/{artilceId}")]
    public async Task<IActionResult> RemoveArticle(int articleId)
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var removePostResult = await _articleService
            .RemoveArticle(token, articleId);
        
        return removePostResult.Match(
            result => Ok(result),
            errors => Problem(errors));
    }

    [HttpPut("editarticletext")]
    public async Task<IActionResult> EditArticleText(EditedArticleText article)
    {
        var token = Request.Headers[HeaderNames.Authorization];
        var editArticleResult = await _articleService
            .EditArticleText(article.ArticleId, 
                         article.NewText, 
                         token);
        
        return editArticleResult.Match(
            result => Ok(result),
            errors => Problem(errors));
    }
    
    [HttpPut("editarticletitle")]
    public async Task<IActionResult> EditArticleTitle(EditedArticleTitle article)
    {
        var token = Request.Headers[HeaderNames.Authorization];
        var editArticleResult = await _articleService
            .EditArticleTitle(article.ArticleId, 
                article.NewTitle, 
                token);
        
        return editArticleResult.Match(
            result => Ok(result),
            errors => Problem(errors));
    }

    [HttpPost("likearticle/{articleId}/{score}")]
    public async Task<IActionResult> LikeArticle(int articleId, int score)
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var likeArticleResult = await _articleService
                .LikeArticle(token, articleId, score);

        return likeArticleResult.Match(
            result => Ok(result),
            errors => Problem(errors));
    }

    [HttpPost("unlikearticle/{articleId}")]
    public async Task<IActionResult> UnLikeArticle(int articleId)
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var unLikeArticleResult = await _articleService
            .UnLikeArticle(token, articleId);

        return unLikeArticleResult.Match(
            result => Ok(result),
            errors => Problem(errors));
    }
    
    [HttpGet("likedarticles")]
    public async Task<IActionResult> LikedArticles()
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var likedArticles = await _articleService
            .GetLikedArticles(token);

        return likedArticles.Match(
            result => Ok(result),
            errors => Problem(errors));
    }
    
    [HttpGet("likes/{articleId}")]
    public async Task<IActionResult> GetArticleLikes(int articleId)
    {
        var likes = await _articleService
            .GetArticleLikes(articleId);

        return likes.Match(
            result => Ok(result),
            errors => Problem(errors));
    }

    [HttpPost("commentarticle")]
    public async Task<IActionResult> CommentArticle(ArticleCommentRequest request)
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var commentResult = await _articleService
            .CommentArticle(token, request.ArticleId, request.Text);
        return commentResult.Match(
            result => Ok(result),
            errors => Problem(errors));
    }

    [HttpDelete("removeComment/{commentId}")]
    public async Task<IActionResult> RemoveComment(int commentId)
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var removeCommentResult = await _articleService
            .RemoveComment(token, commentId);
        return removeCommentResult.Match(
            result => Ok(result),
            errors => Problem(errors));
    }

    [HttpPost("reply")]
    public async Task<IActionResult> ReplyComment(ReplyRequest request)
    {
        string token = Request.Headers[HeaderNames.Authorization];
        var replyResult = await _articleService.Reply(
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
        var removeReplyResult = await _articleService
            .RemoveReply(token, replyId);
        
        return removeReplyResult.Match(
            result => Ok(result),
            errors => Problem(errors));
    }
}