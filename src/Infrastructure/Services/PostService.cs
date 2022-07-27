using Application.Common.Interfaces.Persistence;
using Application.Services;
using Domain.Entities;

namespace Infrastructure.Services;

public class PostService : IPostService
{
    public IPostRepository _postRepository;

    public PostService(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<Post>> GetPosts(string email)
    {
        return await _postRepository.GetPostsByEmail(email);
    }

    public async Task<Post> AddPost(string email, string text, string date)
    {
        var post = new Post()
        {
            Email = email,
            Text = text,
            Date = date
        };
        
        await _postRepository.AddPost(post);

        List<Post> posts = await _postRepository.GetPostsByEmail(email);
        
        return posts.FirstOrDefault(post => post.Date == date);
    }

    public Task RemovePost(int id)
    {
        throw new NotImplementedException();
    }
}