using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Subscribing> Followings => Set<Subscribing>();
    public DbSet<FavouritePost> SavedPosts => Set<FavouritePost>();
    public DbSet<PostLike> Likes => Set<PostLike>();
    public DbSet<PostComment> Comments => Set<PostComment>();
    public DbSet<PostReply> Replies => Set<PostReply>();
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<ArticleLike> ArticleLikes => Set<ArticleLike>();
    public DbSet<ArticleComment> ArticleComments => Set<ArticleComment>();
    public DbSet<ArticleReply> ArticleReplies => Set<ArticleReply>();
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("user");
        modelBuilder.Entity<Post>().ToTable("post");
        modelBuilder.Entity<Subscribing>().ToTable("following");
        modelBuilder.Entity<FavouritePost>().ToTable("savedPosts");
        modelBuilder.Entity<PostLike>().ToTable("likes");
        modelBuilder.Entity<PostComment>().ToTable("Comments");
        modelBuilder.Entity<PostReply>().ToTable("replies");
        modelBuilder.Entity<Article>().ToTable("article");
        modelBuilder.Entity<ArticleLike>().ToTable("articlelikes");
        modelBuilder.Entity<ArticleComment>().ToTable("articlecomments");
        modelBuilder.Entity<ArticleReply>().ToTable("articlereplies");
    }
}