using Application.Models;
using Application.Services;
using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Authentication;
using Domain.Common.Errors;
using ErrorOr;
using Domain.Entities;

namespace Infrastructure.Services;

public class ArticleService : IArticleService
{
    private readonly IArticleRepository _articleRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;

    public ArticleService(IArticleRepository articleRepository, IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _articleRepository = articleRepository;
        _userRepository = userRepository;
    }
    public async Task<ErrorOr<List<ArticleResult>>> GetUserArticles(string token)
    {
        if (_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var articles = await _articleRepository.GetArticlesByUserId(_jwtTokenGenerator.ReadToken(token));
        if (articles.Count == 0)
            return Errors.Articles.ArticlesNotFound;

        List<ArticleResult> userArticles = new();

        foreach (var article in articles)
            userArticles.Add(await MapArticleResult(article));

        return userArticles;
    }

    public async Task<ErrorOr<ArticleResult>> AddArticle(string title, string text, string token)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var articleToAdd = new Article
        {
            User_id = _jwtTokenGenerator.ReadToken(token),
            Title = title,
            Text = text,
            Date = DateTime.UtcNow,
            Likes_count = 0,
            Avg_score = 0
        };
        var article = await _articleRepository.AddArticle(articleToAdd);

        return await MapArticleResult(article);
    }

    public async Task<ErrorOr<Message>> RemoveArticle(string token, int articleId)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var articleToRemove = await _articleRepository.GetArticleById(articleId);

        if (articleToRemove is null)
            return Errors.Articles.ArticleNotFound;

        if (_jwtTokenGenerator.ReadToken(token) != articleToRemove.User_id)
            return Errors.Authentication.WrongToken;

        await _articleRepository.RemoveArticle(articleToRemove);

        return new Message(Correct.Article.ArticleRemoved);
    }

    public async Task<ErrorOr<ArticleResult>> EditArticleText(int articleId, string newText, string token)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var articleToEdit = await _articleRepository.GetArticleById(articleId);
        if (articleToEdit is null)
            return Errors.Articles.ArticleNotFound;

        if (_jwtTokenGenerator.ReadToken(token) != articleToEdit.User_id)
            return Errors.Authentication.WrongToken;

        Article editedArticle = await _articleRepository
            .EditArticleText(articleToEdit, newText);
        
        return await MapArticleResult(editedArticle);
    }
    
    public async Task<ErrorOr<ArticleResult>> EditArticleTitle(int articleId, string newTitle, string token)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var articleToEdit = await _articleRepository.GetArticleById(articleId);
        if (articleToEdit is null)
            return Errors.Articles.ArticleNotFound;

        if (_jwtTokenGenerator.ReadToken(token) != articleToEdit.User_id)
            return Errors.Authentication.WrongToken;

        Article editedArticle = await _articleRepository
            .EditArticleTitle(articleToEdit, newTitle);
        
        return await MapArticleResult(editedArticle);
    }

    public async Task<ErrorOr<ArticleResult>> LikeArticle(string token, int articleId, int score)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var article = await _articleRepository.GetArticleById(articleId);
        if (article is null)
            return Errors.Articles.ArticleNotFound;

        var articleLikes = await _articleRepository.GetArticleLikes(articleId);

        var like = articleLikes.FirstOrDefault(l => l.User_id == _jwtTokenGenerator.ReadToken(token));
        if (like is not null)
            return Errors.Articles.ArticleAlreadyLiked;
        
        double avgScore = Math.Round(
            ((articleLikes.Sum(l => l.Score) + score) 
             / Convert.ToDouble(articleLikes.Count + 1)), 1);
        
        var likedArticle = await _articleRepository
                                .LikeArticle(article, 
                                    _jwtTokenGenerator.ReadToken(token),
                                    score,
                                    avgScore);
        
        return await MapArticleResult(likedArticle);
    }

    public async Task<ErrorOr<ArticleResult>> UnLikeArticle(string token, int articleId)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var article = await _articleRepository.GetArticleById(articleId);
        if (article is null)
            return Errors.Articles.ArticleNotFound;

        var articleLikes = await _articleRepository.GetArticleLikes(articleId);

        var like = articleLikes
                    .FirstOrDefault(l =>
                    l.User_id == _jwtTokenGenerator.ReadToken(token));
        if (like is null)
            return Errors.Articles.ArticleNotLiked;
        double avgScore = Math.Round(
            ((articleLikes.Sum(l => l.Score) - like.Score) 
             / Convert.ToDouble(articleLikes.Count - 1)), 1);
        
        var unLikedArticle = await _articleRepository
                            .UnLikeArticle(
                                article, 
                                _jwtTokenGenerator.ReadToken(token),
                                avgScore);
        
        return await MapArticleResult(unLikedArticle);
    }

    public async Task<ErrorOr<List<ArticleResult>>> GetLikedArticles(string token)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var likes = await _articleRepository.LikedArticles(_jwtTokenGenerator.ReadToken(token));
        if (likes.Count == 0)
            return Errors.Articles.DontLikeAnyArticle;

        List<ArticleResult> likedPosts = new();
        foreach (var like in likes)
        {
            var article = await _articleRepository.GetArticleById(like.Article_id);
            if (article is null)
                continue;
            
            likedPosts.Add(await MapArticleResult(article));
        }

        return likedPosts;
    }

    public async Task<ErrorOr<List<UserLikedPost>>> GetArticleLikes(int articleId)
    {
        var likes = await _articleRepository.GetArticleLikes(articleId);
        if (likes.Count == 0)
            return Errors.Articles.LikesNotFound;

        List<UserLikedPost> users = new();
        foreach (var like in likes)
        {
            var user = await _userRepository.GetUserById(like.User_id);
            if (user is null)
                continue;
            users.Add(new UserLikedPost(
                user.User_id,
                user.Username,
                user.FirstName,
                user.LastName));
        }
        return users;
    }

    public async Task<ErrorOr<ArticleResult>> CommentArticle(string token, int articleId, string text)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var article = await _articleRepository.GetArticleById(articleId);
        if (article is null)
            return Errors.Articles.ArticleNotFound;
        
        var userId = _jwtTokenGenerator.ReadToken(token);
        
        await _articleRepository.CreateComment(new ArticleComment
        {
            Article_id = article.Article_id,
            User_id = userId,
            Text = text,
            Date = DateTime.UtcNow,
            Is_author = (userId == article.User_id)
        });
        
        return await MapArticleResult(article);
    }

    public async Task<ErrorOr<ArticleResult>> Reply(string token, int userId, int commentId, string text)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;
        var comment = await _articleRepository.GetCommentById(commentId);
        if (comment is null)
            return Errors.Articles.CommentNotFound;

        if (await _userRepository.GetUserById(userId) is null)
            return Errors.Posts.CommentNotFound;
        
        var article = await _articleRepository.GetArticleById(comment.Article_id);
        if (article is null)
            return Errors.Articles.ArticleNotFound;
        
        var replierId = _jwtTokenGenerator.ReadToken(token);
        
        await _articleRepository.ReplyComment(new ArticleReply
        {
            User_id = userId,
            Replier_id = replierId,
            Comment_id = commentId,
            Date = DateTime.UtcNow,
            Text = text,
            Is_author = (article.User_id == replierId)
        });

        return await MapArticleResult(article);
    }

    public async Task<ErrorOr<Message>> RemoveReply(string token, int replyId)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var replyToRemove = await _articleRepository.GetReplyById(replyId);

        if (replyToRemove is null)
            return Errors.Articles.ReplyNotFound;

        if (_jwtTokenGenerator.ReadToken(token) != replyToRemove.User_id)
            return Errors.Authentication.WrongToken;

        await _articleRepository.RemoveReply(replyToRemove);

        return new Message(Correct.Post.ReplyRemoved);
    }

    public async Task<ErrorOr<Message>> RemoveComment(string token, int commentId)
    {
        if (_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var commentToRemove = await _articleRepository.GetCommentById(commentId);

        if (commentToRemove is null)
            return Errors.Articles.CommentNotFound;

        if (_jwtTokenGenerator.ReadToken(token) != commentToRemove.User_id)
            return Errors.Authentication.WrongToken;

        await _articleRepository.RemoveComment(commentToRemove);

        return new Message(Correct.Post.CommentRemoved);
    }
    
    private async Task<ArticleResult> MapArticleResult(Article article)
    {
        List<ArticleCommentResponse> comments = SortCommentsByDate(
            await MapArticleCommentResult(await _articleRepository.GetComments(article.Article_id)));
        
        return new ArticleResult(
            article.Article_id,
            article.User_id,
            article.Title,
            article.Text,
            article.Date,
            comments,
            article.Likes_count,
            article.Avg_score);
    }

    private async Task<List<ArticleCommentResponse>> MapArticleCommentResult(List<ArticleComment> dbArticleComments)
    {
        List<ArticleCommentResponse> articleComments = new();
        foreach (var dbComment in dbArticleComments)
        {
            List<ArticleReplyResponse> articleReplies = await MapArticleReplyResult(
                await _articleRepository.GetReplies(dbComment.Comment_id));
            
            articleComments.Add(new ArticleCommentResponse(
                dbComment.Comment_id,
                dbComment.Article_id,
                dbComment.User_id,
                dbComment.Text,
                dbComment.Date,
                articleReplies,
                dbComment.Is_author));
        }

        return articleComments;
    }

    private async Task<List<ArticleReplyResponse>> MapArticleReplyResult(List<ArticleReply> dbArticleReplies)
    {
        List<ArticleReplyResponse> articleReplies = new();

        foreach (var dbReply in dbArticleReplies)
            articleReplies.Add(new ArticleReplyResponse(
                dbReply.Reply_id,
                dbReply.Comment_id,
                (await _userRepository.GetUserById(dbReply.User_id))!,
                (await _userRepository.GetUserById(dbReply.Replier_id))!,
                dbReply.Text,
                dbReply.Date,
                dbReply.Is_author));
        
        return SortRepliesByDate(articleReplies);
    }

    private List<ArticleReplyResponse> SortRepliesByDate(List<ArticleReplyResponse> replies)
        => replies
            .OrderBy(r => r.Date)
            .ToList();

    private List<ArticleCommentResponse> SortCommentsByDate(List<ArticleCommentResponse> comments)
        => comments.OrderBy(c => c.Date).ToList();
}