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
    public async Task<ErrorOr<List<ArticleResult>>> GetArticles(string token)
    {
        if(token == String.Empty)
            return Errors.Authentication.TokenNotFound;
        
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var articles = await _articleRepository.GetArticlesByUserId(_jwtTokenGenerator.ReadToken(token));
        if(articles.Count == 0)
            return Errors.Articles.ArticlesNotFound;
        
        List<ArticleResult> userArticles = new();

        foreach(var article in articles)
        {
            var dbComments = await _articleRepository.GetComments(article.Id);
            List<ArticleCommentResponse> comments = new();
            foreach(var item in dbComments)
            {
                var dbReplies = SortReplies(await _articleRepository.GetReplies(item.Id));
                var replies = new List<ArticleReplyResponse>();

                foreach (var dbReply in dbReplies)
                    replies.Add(new ArticleReplyResponse(
                        dbReply.Id,
                        dbReply.ArticleCommentId,
                        (await _userRepository.GetUserById(dbReply.UserId))!,
                        (await _userRepository.GetUserById(dbReply.ReplierId))!,
                        dbReply.Text,
                        dbReply.Data));  
                comments.Add(new ArticleCommentResponse(
                    item.Id,
                    item.ArticleId,
                    item.UserId,
                    item.Text,
                    item.Date,
                    replies));
            }
            userArticles.Add(new ArticleResult(
                article.Id,
                article.UserId,
                article.Text,
                article.Date,
                SortComments(comments),
                article.Likes));
        }

        return userArticles;
    }

    public async Task<ErrorOr<ArticleResult>> AddArticle(string text, string token)
    {
        if(token == string.Empty)
            return Errors.Authentication.TokenNotFound;
        
        if(!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;
        
        var articleToAdd = new Article
        {
            UserId = _jwtTokenGenerator.ReadToken(token),
            Text = text,
            Date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"),
            Likes = 0
        };
        var article = await _articleRepository.AddArticle(articleToAdd);
        var dbComments = await _articleRepository.GetComments(article.Id);
        List<ArticleCommentResponse> comments = new();

        foreach(var item in dbComments)
        {
            var dbReplies = SortReplies(await _articleRepository.GetReplies(item.Id));
            var replies = new List<ArticleReplyResponse>();
            
            foreach (var dbReply in dbReplies)
                replies.Add(new ArticleReplyResponse(
                    dbReply.Id,
                    dbReply.ArticleCommentId,
                    (await _userRepository.GetUserById(dbReply.UserId))!,
                    (await _userRepository.GetUserById(dbReply.ReplierId))!,
                    dbReply.Text,
                    dbReply.Data
                ));
            
            comments.Add(new ArticleCommentResponse(
                item.Id,
                item.ArticleId,
                item.UserId,
                item.Text,
                item.Date, 
                replies));
        }

        return new ArticleResult(
            article.Id,
            article.UserId,
            article.Text,
            article.Date,
            SortComments(comments),
            0);
    }

    public async Task<ErrorOr<Message>> RemoveArticle(string token, int articleId)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var articleToRemove = await _articleRepository.GetArticleById(articleId);

        if (articleToRemove is null)
            return Errors.Articles.ArticleNotFound;
        
        if (_jwtTokenGenerator.ReadToken(token) != articleToRemove.UserId)
            return Errors.Authentication.WrongToken;
        
        await _articleRepository.RemoveArticle(articleToRemove);

        return new Message(Correct.Article.ArticleRemoved);
    }

    public async Task<ErrorOr<ArticleResult>> EditArticle(int articleId, string newText, string token)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var articleToEdit = await _articleRepository.GetArticleById(articleId);
        if (articleToEdit is null)
            return Errors.Articles.ArticleNotFound;

        if (_jwtTokenGenerator.ReadToken(token) != articleToEdit.UserId)
            return Errors.Authentication.WrongToken;

        var editedArtilce = await _articleRepository.EditArticle(articleToEdit, newText);
        var dbComments = await _articleRepository.GetComments(editedArtilce.Id);
        
        List<ArticleCommentResponse> comments = new();
        
        foreach (var item in dbComments)
        {
            var dbReplies = SortReplies(await _articleRepository.GetReplies(item.Id));
            var replies = new List<ArticleReplyResponse>();
            
            foreach (var dbReply in dbReplies)
                replies.Add(new ArticleReplyResponse(
                    dbReply.Id,
                    dbReply.ArticleCommentId,
                    (await _userRepository.GetUserById(dbReply.UserId))!,
                    (await _userRepository.GetUserById(dbReply.ReplierId))!,
                    dbReply.Text,
                    dbReply.Data));
            
            comments.Add(new ArticleCommentResponse(
                item.Id,
                item.ArticleId,
                item.UserId,
                item.Text,
                item.Date, 
                replies));
        }
        return new ArticleResult(
            editedArtilce.Id,
            editedArtilce.UserId,
            editedArtilce.Text,
            editedArtilce.Date,
            SortComments(comments),
            editedArtilce.Likes);
    }

    public async Task<ErrorOr<ArticleResult>> LikeArticle(string token, int articleId)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var article = await _articleRepository.GetArticleById(articleId);
        if (article is null)
            return Errors.Articles.ArticleNotFound;

        var articleLikes = await _articleRepository.GetArticleLikes(articleId);

        var like = articleLikes.FirstOrDefault(l => l.UserId == _jwtTokenGenerator.ReadToken(token));
        if (like is not null)
            return Errors.Articles.ArticleAlreadyLiked;
        

        var likedArticle = await _articleRepository.LikeArticle(article, _jwtTokenGenerator.ReadToken(token));
        var dbComments = await _articleRepository.GetComments(likedArticle.Id);
        List<ArticleCommentResponse> comments = new();
        
        foreach (var item in dbComments)
        {
            var dbReplies = SortReplies(await _articleRepository.GetReplies(item.Id));
            var replies = new List<ArticleReplyResponse>();
            
            foreach (var dbReply in dbReplies)
                replies.Add(new ArticleReplyResponse(
                    dbReply.Id,
                    dbReply.ArticleCommentId,
                    (await _userRepository.GetUserById(dbReply.UserId))!,
                    (await _userRepository.GetUserById(dbReply.ReplierId))!,
                    dbReply.Text,
                    dbReply.Data));
            
            comments.Add(new ArticleCommentResponse(
                item.Id,
                item.ArticleId,
                item.UserId,
                item.Text,
                item.Date, 
                replies));
        }
        return new ArticleResult(
            article.Id,
            article.UserId,
            article.Text,
            article.Date,
            SortComments(comments),
            article.Likes);
    }

    public async Task<ErrorOr<ArticleResult>> UnLikeArticle(string token, int articleId)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var article = await _articleRepository.GetArticleById(articleId);
        if (article is null)
            return Errors.Articles.ArticleNotFound;

        var articleLikes = await _articleRepository.GetArticleLikes(articleId);

        var like = articleLikes
                    .FirstOrDefault(l => 
                    l.UserId == _jwtTokenGenerator.ReadToken(token));
        if(like is not null)
            return Errors.Articles.ArticleAlreadyLiked;

        var unLikedArticle = await _articleRepository.UnLikeArticle(article, _jwtTokenGenerator.ReadToken(token));
        var dbComments = await _articleRepository.GetComments(unLikedArticle.Id);
        List<ArticleCommentResponse> comments = new();
        
        foreach (var item in dbComments)
        {
            var dbReplies = SortReplies(await _articleRepository.GetReplies(item.Id));
            var replies = new List<ArticleReplyResponse>();
            
            foreach (var dbReply in dbReplies)
                replies.Add(new ArticleReplyResponse(
                    dbReply.Id,
                    dbReply.ArticleCommentId,
                    (await _userRepository.GetUserById(dbReply.UserId))!,
                    (await _userRepository.GetUserById(dbReply.ReplierId))!,
                    dbReply.Text,
                    dbReply.Data));
            
            comments.Add(new ArticleCommentResponse(
                item.Id,
                item.ArticleId,
                item.UserId,
                item.Text,
                item.Date, 
                replies));
        }
        return new ArticleResult(
            article.Id,
            article.UserId,
            article.Text,
            article.Date,
            SortComments(comments),
            article.Likes);
    }

    public async Task<ErrorOr<List<ArticleResult>>> GetLikedArticles(string token)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var likes = await _articleRepository.LikedArticles(_jwtTokenGenerator.ReadToken(token));
        if (likes.Count == 0)
            return Errors.Articles.DontLikeAnyArticle;

        List<ArticleResult> likedPosts = new();
        foreach (var like in likes)
        {
            var article = await _articleRepository.GetArticleById(like.ArticleId);
            if (article is null)
                continue;
            
            var dbComments = await _articleRepository.GetComments(article.Id);
            List<ArticleCommentResponse> comments = new();
        
            foreach (var item in dbComments)
            {
                var dbReplies = SortReplies(await _articleRepository.GetReplies(item.Id));
                var replies = new List<ArticleReplyResponse>();
            
                foreach (var dbReply in dbReplies)
                    replies.Add(new ArticleReplyResponse(
                        dbReply.Id,
                        dbReply.ArticleCommentId,
                        (await _userRepository.GetUserById(dbReply.UserId))!,
                        (await _userRepository.GetUserById(dbReply.ReplierId))!,
                        dbReply.Text,
                        dbReply.Data));
            
                comments.Add(new ArticleCommentResponse(
                    item.Id,
                    item.ArticleId,
                    item.UserId,
                    item.Text,
                    item.Date, 
                    replies));
            }
            likedPosts.Add(new ArticleResult(
                article.Id,
                article.UserId,
                article.Text,
                article.Date,
                SortComments(comments),
                article.Likes));
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
            var user = await _userRepository.GetUserById(like.UserId);
            if (user is null)
                continue;
            users.Add(new UserLikedPost(
                user.Id,
                user.Email,
                user.Username,
                user.FirstName,
                user.LastName,
                user.Status));
        }
        return users;
    }

    public async Task<ErrorOr<ArticleResult>> CommentArticle(string token, int articleId, string text)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var article = await _articleRepository.GetArticleById(articleId);
        if (article is null)
            return Errors.Articles.ArticleNotFound;

        await _articleRepository.CreateComment(new ArticleComment
        {
            ArticleId = article.Id,
            Date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"),
            Text = text,
            UserId = _jwtTokenGenerator.ReadToken(token)
        });
        
        var dbComments = await _articleRepository.GetComments(article.Id);
        List<ArticleCommentResponse> comments = new();
        
        foreach (var item in dbComments)
        {
            var dbReplies = SortReplies(await _articleRepository.GetReplies(item.Id));
            var replies = new List<ArticleReplyResponse>();
            
            foreach (var dbReply in dbReplies)
                replies.Add(new ArticleReplyResponse(
                    dbReply.Id,
                    dbReply.ArticleCommentId,
                    (await _userRepository.GetUserById(dbReply.UserId))!,
                    (await _userRepository.GetUserById(dbReply.ReplierId))!,
                    dbReply.Text,
                    dbReply.Data));
            
            comments.Add(new ArticleCommentResponse(
                item.Id,
                item.ArticleId,
                item.UserId,
                item.Text,
                item.Date, 
                replies));
        }
        return new ArticleResult(
            article.Id,
            article.UserId,
            article.Text,
            article.Date,
            SortComments(comments),
            article.Likes);
    }

    public async Task<ErrorOr<ArticleResult>> Reply(string token, int userId, int commentId, string text)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;
        var comment = await _articleRepository.GetCommentById(commentId);
        if (comment is null)
            return Errors.Articles.CommentNotFound;

        if (await _userRepository.GetUserById(userId) is null)
            return Errors.Posts.CommentNotFound;

        await _articleRepository.ReplyComment(new ArticleReply
        {
            UserId = userId,
            ReplierId = _jwtTokenGenerator.ReadToken(token),
            ArticleCommentId = commentId,
            Data = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"),
            Text = text
        });
        var article = await _articleRepository.GetArticleById(comment!.ArticleId);
        
        var dbComments = await _articleRepository.GetComments(article!.Id);
        List<ArticleCommentResponse> comments = new();
        
        foreach (var item in dbComments)
        {
            var dbReplies = SortReplies(await _articleRepository.GetReplies(item.Id));
            var replies = new List<ArticleReplyResponse>();
            
            foreach (var dbReply in dbReplies)
                replies.Add(new ArticleReplyResponse(
                    dbReply.Id,
                    dbReply.ArticleCommentId,
                    (await _userRepository.GetUserById(dbReply.UserId))!,
                    (await _userRepository.GetUserById(dbReply.ReplierId))!,
                    dbReply.Text,
                    dbReply.Data));
            
            comments.Add(new ArticleCommentResponse(
                item.Id,
                item.ArticleId,
                item.UserId,
                item.Text,
                item.Date, 
                replies));
        }
        return new ArticleResult(
            article.Id,
            article.UserId,
            article.Text,
            article.Date,
            SortComments(comments),
            article.Likes);
    }

    public async Task<ErrorOr<Message>> RemoveReply(string token, int replyId)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;
        
        var replyToRemove = await _articleRepository.GetReplyById(replyId);
        
        if (replyToRemove is null)
            return Errors.Articles.ReplyNotFound;

        if (_jwtTokenGenerator.ReadToken(token) != replyToRemove.UserId)
            return Errors.Authentication.WrongToken;
        
        await _articleRepository.RemoveReply(replyToRemove);

        return new Message(Correct.Post.ReplyRemoved);
    }

    public async Task<ErrorOr<Message>> RemoveComment(string token, int commentId)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;
        
        var commentToRemove = await _articleRepository.GetCommentById(commentId);
        
        if (commentToRemove is null)
            return Errors.Articles.CommentNotFound;

        if (_jwtTokenGenerator.ReadToken(token) != commentToRemove.UserId)
            return Errors.Authentication.WrongToken;
        
        await _articleRepository.RemoveComment(commentToRemove);

        return new Message(Correct.Post.CommentRemoved);
    }

    private List<ArticleReply> SortReplies(List<ArticleReply> replies)
        => replies
            .OrderBy(r => r.Data)
            .ToList();

    private List<ArticleCommentResponse> SortComments(List<ArticleCommentResponse> comments)
        => comments.OrderBy(c => c.Date).ToList();
}