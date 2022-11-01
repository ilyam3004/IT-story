using Application.Common.Interfaces.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext context)
    {
        _db = context;
    }

    public async Task<User?> GetByEmail(string email)
        => await _db.Users.FirstOrDefaultAsync(user => user.Email == email);

    public async Task<User?> GetByUserName(string userName)
        => await _db.Users.FirstOrDefaultAsync(user => user.Username == userName);

    public async Task<User?> GetUserById(int id)
        => await _db.Users.FirstOrDefaultAsync(user => user.User_id == id);

    public async Task AddUser(User user)
    {
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
    }
}