using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{ 
    public DbSet<User> Users => Set<User>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Following> Followings => Set<Following>();
    public DbSet<SavedPost> SavedPosts => Set<SavedPost>();
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("user");
        modelBuilder.Entity<Post>().ToTable("post");
        modelBuilder.Entity<Following>().ToTable("following");
        modelBuilder.Entity<SavedPost>().ToTable("savedPosts");
    }
}