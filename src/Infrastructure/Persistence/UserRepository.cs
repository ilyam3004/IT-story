using Application.Common.Interfaces.Persistence;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Presistence;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _db;

    public UserRepository(ApplicationDbContext context)
    {
        _db = context;
    }

    public async Task<User?> GetByEmail(string email)
    {
        var finduser = await _db.Users.FirstOrDefaultAsync(user => user.email == email);
        return finduser;
    }

    public async Task Add(User user)
    {
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
    }
}