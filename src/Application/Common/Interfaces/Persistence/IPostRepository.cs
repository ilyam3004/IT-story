using Domain.Entities;

namespace Application.Common.Interfaces.Persistence;

public interface IPostRepository
{
    Task<List<Post>> GetPostsByEmail(string email);
    Task AddPost(Post post);
    Task RemovePost(int id);
}