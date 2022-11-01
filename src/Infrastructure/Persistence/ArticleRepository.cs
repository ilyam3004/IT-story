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
        => _db.Articles.Where(a => a.User_id == userId).ToListAsync();

    public async Task<Article> AddArticle(Article article)
    {
        await _db.Articles.AddAsync(article);
        await _db.SaveChangesAsync();
        return (await _db.Articles.FirstOrDefaultAsync(a => a.Article_id == article.Article_id))!;
    }

    public async Task<Article> EditArticleText(Article article, string newText)
    {
        article!.Text = newText;
        await _db.SaveChangesAsync();
        return article;
    }
    
    public async Task<Article> EditArticleTitle(Article article, string newTitle)
    {
        article!.Title = newTitle;
        await _db.SaveChangesAsync();
        return article;
    }
    
    public async Task<Article?> GetArticleById(int articleId)
        => await _db.Articles.FirstOrDefaultAsync(a => a.Article_id == articleId);

    public async Task RemoveArticle(Article article)
    {
        _db.Articles.Remove(article);
        await _db.SaveChangesAsync();
    }

    public async Task<Article> LikeArticle(Article article, int userId, int score, double avgScore)
    {
        await _db.ArticleLikes.AddAsync(new ArticleLike
        {
            User_id = userId,
            Article_id = article.Article_id,
            Score = score
        });
        article.Likes_count++;
        article.Avg_score = avgScore;
        return article;
    }

    public async Task<Article> UnLikeArticle(Article article, int userId, double avgScore)
    {
        var likeToRemove = await GetLikeByArticleId(userId, article.Article_id);
        _db.ArticleLikes.Remove(likeToRemove);
        article.Likes_count--;
        article.Avg_score = avgScore;
        await _db.SaveChangesAsync();
        return article;
    }

    public async Task<List<ArticleLike>> LikedArticles(int userId)
        => await _db.ArticleLikes
        .Where(like => like.User_id == userId)
        .ToListAsync();

    public async Task<ArticleLike> GetLikeByArticleId(int userId, int articleId)
        => (await _db.ArticleLikes
        .FirstOrDefaultAsync(like => like.User_id == userId &&
                                     like.Article_id == articleId))!;

    public async Task<List<ArticleLike>> GetArticleLikes(int articleId)
        => await _db.ArticleLikes
            .Where(like => like.Article_id == articleId)
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
            .Where(comment => comment.Article_id == articleId)
            .ToListAsync();

    public Task<ArticleComment?> GetCommentById(int commentId)
        => _db.ArticleComments
            .FirstOrDefaultAsync(comment => comment.Comment_id == commentId);

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
            .Where(reply => reply.Comment_id == commentId)
            .ToListAsync();

    public async Task<ArticleReply?> GetReplyById(int replyId)
        => await _db.ArticleReplies
            .FirstOrDefaultAsync(reply => reply.Reply_id == replyId);
}