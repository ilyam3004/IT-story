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

    public PostService(IPostRepository postRepository, IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository)
    {
        _postRepository = postRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<List<PostResult>>> GetPosts(string token)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        int id = _jwtTokenGenerator.ReadToken(token);

        var posts = await _postRepository.GetPostsByUserId(id);
        if (posts.Count == 0)
            return Errors.Posts.PostsNotFound;

        List<PostResult> userPosts = new();
        foreach (var post in posts)
            userPosts.Add(new PostResult(
                post.Id,
                post.UserId,
                post.Text,
                post.Date,
                await _postRepository.GetComments(post.Id),
                post.Likes));

        return userPosts;
    }

    public async Task<ErrorOr<PostResult>> AddPost(string text, string token)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        int userId = _jwtTokenGenerator.ReadToken(token);

        var post = new Post()
        {
            UserId = userId,
            Text = text,
            Date = DateTime.Now.ToString("dd MMMM yyyy, HH:mm",CultureInfo.CreateSpecificCulture("en-US")),
            Likes = 0
        };

        await _postRepository.AddPost(post);

        return new PostResult(
            post.Id,
            post.UserId,
            post.Text,
            post.Date,
            await _postRepository.GetComments(post.Id),
            0);
    }

    public async Task<ErrorOr<PostResult>> RemovePost(int postId)
    {
        var postToRemove = await _postRepository.GetPostById(postId);

        if (postToRemove is null)
            return Errors.Posts.PostNotFound;

        await _postRepository.RemovePost(postToRemove);

        return new PostResult(
            postToRemove.Id,
            postToRemove.UserId,
            postToRemove.Text,
            postToRemove.Date,
            await _postRepository.GetComments(postId),
            postToRemove.Likes);
    }
    
    public async Task<ErrorOr<PostResult>> EditPost(int postId, string newText)
    {
        var postToEdit = await _postRepository.GetPostById(postId);

        if (postToEdit is null)
            return Errors.Posts.PostNotFound;

        var editedPost = await _postRepository.EditPost(postToEdit, newText);

        return new PostResult(
            editedPost.Id,
            editedPost.UserId,
            editedPost.Text,
            editedPost.Date,
            await _postRepository.GetComments(postId),
            editedPost.Likes);
    }

    public async Task<ErrorOr<List<PostResult>>> GetSavedPosts(string token)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;
        
        var posts = await _postRepository.GetSavedPosts(_jwtTokenGenerator.ReadToken(token));

        if (posts.Count == 0)
            return Errors.Posts.SavedPostsNotfound;

        List<PostResult> savedPosts = new();
        foreach (var item in posts)
        {
            var post = await _postRepository.GetPostById(item.PostId);
            if (post is null) 
                continue;
            savedPosts.Add(new PostResult(
                post.Id,
                post.UserId,
                post.Text,
                post.Date,
                await _postRepository.GetComments(post.Id),
                post.Likes));
        }

        return savedPosts;
    }

    public async Task<ErrorOr<PostResult>> SavePost(string token, int postId)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var postToSave = await _postRepository.GetPostById(postId);
        
        if (postToSave is null)
            return Errors.Posts.PostNotFound;
        
        var savedPost = await _postRepository.GetSavedPostById(postId);
        
        if (savedPost is not null)
            return Errors.Posts.PostAlreadySaved;
        
        
        await _postRepository.SavePost(new SavedPost
            {
                PostId = postToSave.Id,
                UserId = postToSave.UserId
            });

        return new PostResult(
                postToSave.Id,
                postToSave.UserId,
                postToSave.Text,
                postToSave.Date,
                await _postRepository.GetComments(postId),
                postToSave.Likes);
    }

    public async  Task<ErrorOr<PostResult>> UnSavePost(string token, int postId)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;
        
        var postToReturn = await _postRepository.GetPostById(postId);
        if (postToReturn is null)
            return Errors.Posts.PostNotFound;
        
        var postToUnSave = await _postRepository.GetSavedPostById(postId);
        if (postToUnSave is null)
            return Errors.Posts.SavedPostNotfound;
        
        await _postRepository.UnSavePost(postToUnSave);
        return new PostResult(
            postToReturn.Id,
            postToReturn.UserId,
            postToReturn.Text,
            postToReturn.Date,
            await _postRepository.GetComments(postId),
            postToReturn.Likes);
    }

    public async Task<ErrorOr<PostResult>> LikePost(string token, int postId)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;
        
        var post = await _postRepository.GetPostById(postId);
        if (post is null)
            return Errors.Posts.PostNotFound;

        var likedPost = await _postRepository.LikePost(post);
        return new PostResult(
            likedPost.Id,
            likedPost.UserId,
            likedPost.Text,
            likedPost.Date,
            await _postRepository.GetComments(post.Id),
            likedPost.Likes);
    }
    public async Task<ErrorOr<PostResult>> UnLikePost(string token, int postId)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;
        
        var post = await _postRepository.GetPostById(postId);
        if (post is null)
            return Errors.Posts.PostNotFound;

        var unLikedPost = await _postRepository.UnLikePost(post);
        return new PostResult(
            unLikedPost.Id,
            unLikedPost.UserId,
            unLikedPost.Text,
            unLikedPost.Date,
            await _postRepository.GetComments(post.Id),
            unLikedPost.Likes);
    }

    public async Task<ErrorOr<List<PostResult>>> GetLikedPosts(string token)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var likes = await _postRepository.LikedPosts(_jwtTokenGenerator.ReadToken(token));
        if (likes.Count == 0)
            return Errors.Posts.DontLikeAnyPost;

        List<PostResult> likedPosts = new();
        foreach (var like in likes)
        {
            var post = await _postRepository.GetPostById(like.PostId);
            if(post is null)
                continue;
            likedPosts.Add(new PostResult(
                post.Id,
                post.UserId,
                post.Text,
                post.Date,
                await _postRepository.GetComments(post.Id),
                post.Likes));
        }

        return likedPosts;
    }

    public async Task<ErrorOr<List<UserLikedPost>>> GetPostLikes(int postId)
    {
        var likes = await _postRepository.GetPostLikes(postId);
        if (likes.Count == 0)
            return Errors.Posts.LikesNotFound;

        List<UserLikedPost> users = new();
        foreach (var like in likes)
        {
            var user = await _userRepository.GetUserById(like.UserId);
            if(user is null)
                continue;
            users.Add(new UserLikedPost(
                user.Id,
                user.Email,
                user.Username,
                user.FirstName,
                user.LastName,
                user.Status));
        }

        return users;
    }

    public async Task<ErrorOr<PostResult>> CommentPost(string token, int postId, string text)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;
        
        var post = await _postRepository.GetPostById(postId);
        if (post is null)
            return Errors.Posts.PostNotFound;

        await _postRepository.CreateComment(new Comment
        {
            PostId = post.Id,
            Date = DateTime.Now.ToString("dd MMMM yyyy, HH:mm", CultureInfo.CreateSpecificCulture("en-US")),
            Text = text,
            UserId = _jwtTokenGenerator.ReadToken(token)
        });
        return new PostResult(
            post.Id,
            post.UserId,
            post.Text,
            post.Date,
            await _postRepository.GetComments(post.Id),
            post.Likes);
    }
}