using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Persistence;

public class AppDbContext : DbContext
{ 
    public DbSet<User> Users => Set<User>();
    public DbSet<Post> Posts => Set<Post>();
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("user");
        modelBuilder.Entity<Post>().ToTable("post");
    }
}