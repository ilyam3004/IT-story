using Domain.Entities;

namespace Application.Common.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
    int ReadToken(string token);
    bool CanReadToken(string token);
}