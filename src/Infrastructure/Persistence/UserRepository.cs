using Application.Common.Interfaces.Persistence;
using Domain.Entities;

namespace Infrastructure.Presistence;

public class UserRepository : IUserRepository
{
    //TODO Implement Db Context and add then implement IUserRepository
    User? IUserRepository.GetByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public void Add(User user)
    {
        throw new NotImplementedException();
    }
}