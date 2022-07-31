using System.Globalization;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Persistence;
using Application.Models;
using Application.Services;
using Domain.Common.Errors;
using Domain.Entities;
using ErrorOr;

namespace Infrastructure.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;

    public PostService(IPostRepository postRepository, IJwtTokenGenerator jwtTokenGenerator,
        IUserRepository userRepository)
    {
        _postRepository = postRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<List<PostResult>>> GetPosts(string token)
    {
        if (token == String.Empty)
            return Errors.Posts.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Posts.WrongToken;

        int id = _jwtTokenGenerator.ReadToken(token);

        var posts = await _postRepository.GetPostsByUserId(id);
        if (posts.Count == 0)
            return Errors.Posts.PostsNotFound;

        List<PostResult> userPosts = new();
        foreach (var item in posts)
            userPosts.Add(new PostResult(
                item.Id,
                item.UserId,
                item.Text,
                item.Date));

        return userPosts;
    }

    public async Task<ErrorOr<PostResult>> AddPost(string text, string token)
    {
        if (token == String.Empty)
            return Errors.Posts.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Posts.WrongToken;

        int userId = _jwtTokenGenerator.ReadToken(token);

        var post = new Post()
        {
            UserId = userId,
            Text = text,
            Date = DateTime.Now.ToString("dd MMMM yyyy, HH:mm",CultureInfo.CreateSpecificCulture("en-US"))
        };

        await _postRepository.AddPost(post);

        List<Post> posts = await _postRepository.GetPostsByUserId(userId);

        return new PostResult(
            post.Id,
            post.UserId,
            post.Text,
            post.Date);
    }

    public async Task<ErrorOr<PostResult>> RemovePost(int postId)
    {
        var postToRemove = await _postRepository.GetPostById(postId);

        if (postToRemove is null)
            return Errors.Posts.PostNotFound;

        await _postRepository.RemovePost(postId);

        return new PostResult(
            postToRemove.Id,
            postToRemove.UserId,
            postToRemove.Text,
            postToRemove.Date);
    }
    
    public async Task<ErrorOr<PostResult>> EditPost(int postId, string newText)
    {
        var postToEdit = await _postRepository.GetPostById(postId);

        if (postToEdit is null)
            return Errors.Posts.PostNotFound;

        var editedPost = await _postRepository.EditPost(postId, newText);

        return new PostResult(
            editedPost.Id,
            editedPost.UserId,
            editedPost.Text,
            editedPost.Date);
    }
}