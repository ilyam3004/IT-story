using Domain.Entities;

namespace Application.Common.Interfaces.Persistence;

public interface IUserRepository
{
    Task<User?> GetByEmail(string email);
    Task<User?> GetUserById(int id);
    Task Add(User user);
}