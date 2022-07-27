using Application.Common.Interfaces.Persistence;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Presistence;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _db;

    public UserRepository(UserDbContext context)
    {
        _db = context;
    }

    public async Task<User?> GetByEmail(string email)
        => await _db.Users.FirstOrDefaultAsync(user => user.Email == email);
    
    public async Task Add(User user)
    {
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
    }
}