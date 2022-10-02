using Application.Common.Interfaces.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AritcleRepository : IArticleRepository
{
    private readonly AppDbContext _db;

    public AritcleRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<Article>> GetArticlesByUserId(int userId)
        => _db.Articles.Where(a => a.UserId == userId).ToListAsync();

    public async Task<Article> AddArticle(Article article)
    {
        await _db.Articles.AddAsync(article);
        await _db.SaveChangesAsync();
        return (await _db.Articles.FirstOrDefaultAsync(a => a.Id == article.Id))!;
    }

    public async Task<Article> EditArticle(Article article, string newText)
    {
        article!.Text = newText;
        await _db.SaveChangesAsync();
        return article;
    }
    public async Task<Article?> GetArticleById(int articleId)
        => await _db.Articles.FirstOrDefaultAsync(a => a.Id == articleId);

    public async Task RemoveArticle(Article article)
    {
        _db.Articles.Remove(article);
        await _db.SaveChangesAsync();
    }

    public async Task<Article> LikeArticle(Article article, int userId)
    {
        article.Likes++;
        await _db.ArticleLikes.AddAsync(new ArticleLike
        {
            UserId = userId,
            ArticleId = article.Id
        });
        return article;
    }

    public async Task<Article> UnLikeArticle(Article article, int userId)
    {
        article.Likes--;
        var likeToRemove = await GetLikeByArticleId(userId, article.Id);
        _db.ArticleLikes.Remove(likeToRemove);
        await _db.SaveChangesAsync();
        return article;
    }
        
    public async Task<List<ArticleLike>> LikedArticles(int userId)
        => await  _db.ArticleLikes
        .Where(like => like.UserId == userId)
        .ToListAsync();

    public async Task<ArticleLike> GetLikeByArticleId(int userId, int articleId)
        => (await _db.ArticleLikes
        .FirstOrDefaultAsync(like => like.UserId == userId && 
                                     like.ArticleId == articleId))!;

    public async Task<List<ArticleLike>> GetArticleLikes(int articleId)
        => await _db.ArticleLikes
            .Where(like => like.ArticleId == articleId)
            .ToListAsync();

    public async Task CreateComment(ArticleComment comment)
    {
        await _db.ArticleComments.AddAsync(comment);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveComment(ArticleComment comment)
    {
        _db.ArticleComments.Remove(comment);
        await _db.SaveChangesAsync();
    }

    public async Task<List<ArticleComment>> GetComments(int articleId)
        => await _db.ArticleComments
            .Where(comment => comment.ArticleId == articleId)
            .ToListAsync();

    public Task<ArticleComment?> GetCommentById(int commentId)
        => _db.ArticleComments
            .FirstOrDefaultAsync(comment => comment.Id == commentId);
    
    public async Task ReplyComment(ArticleReply reply)
    {
        await _db.ArticleReplies.AddAsync(reply);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveReply(ArticleReply reply)
    {
        _db.ArticleReplies.Remove(reply);
        await _db.SaveChangesAsync();
    }

    public async Task<List<ArticleReply>> GetReplies(int commentId)
        => await _db.ArticleReplies
            .Where(reply => reply.ArticleCommentId == commentId)
            .ToListAsync();

    public async Task<ArticleReply?> GetReplyById(int replyId)
        => await _db.ArticleReplies
            .FirstOrDefaultAsync(reply => reply.Id == replyId);
}