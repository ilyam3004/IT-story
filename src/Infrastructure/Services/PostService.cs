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
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var posts = await _postRepository.GetPostsByUserId(_jwtTokenGenerator.ReadToken(token));
        if (posts.Count == 0)
            return Errors.Posts.PostsNotFound;

        List<PostResult> userPosts = new();
        foreach (var post in posts)
        {
            var dbComments = await _postRepository.GetComments(post.Id);
            List<CommentResult> comments = new();
            foreach (var item in dbComments)
            {
                var replies = SortReplies(await _postRepository.GetReplies(item.Id));
                comments.Add(new CommentResult(
                    item.Id,
                    item.PostId,
                    item.UserId,
                    item.Text,
                    item.Date,
                    replies));
            }

            userPosts.Add(new PostResult(
                post.Id,
                post.UserId,
                post.Text,
                post.Date,
                SortComments(comments),
                post.Likes));
        }

        return userPosts;
    }

    public async Task<ErrorOr<PostResult>> AddPost(string text, string token)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var postToAdd = new Post()
        {
            UserId = _jwtTokenGenerator.ReadToken(token),
            Text = text,
            Date = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
            Likes = 0
        };
        var post = await _postRepository.AddPost(postToAdd);
        var dbComments = await _postRepository.GetComments(post.Id);
        List<CommentResult> comments = new();

        foreach (var comment in dbComments)
        {
            var replies = SortReplies(await _postRepository.GetReplies(comment.Id));
            comments.Add(new CommentResult(
                comment.Id,
                comment.PostId,
                comment.UserId,
                comment.Text,
                comment.Date,
                replies));
        }

        return new PostResult(
            post.Id,
            post.UserId,
            post.Text,
            post.Date,
            SortComments(comments),
            0);
    }

    public async Task<ErrorOr<string>> RemovePost(string token, int postId)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;
        
        var postToRemove = await _postRepository.GetPostById(postId);
        
        if (postToRemove is null)
            return Errors.Posts.PostNotFound;

        if (_jwtTokenGenerator.ReadToken(token) != postToRemove.UserId)
            return Errors.Authentication.WrongToken;
        
        await _postRepository.RemovePost(postToRemove);

        return Correct.Post.PostRemoved;
    }

    public async Task<ErrorOr<PostResult>> EditPost(int postId, string newText)
    {
        var postToEdit = await _postRepository.GetPostById(postId);

        if (postToEdit is null)
            return Errors.Posts.PostNotFound;

        var editedPost = await _postRepository.EditPost(postToEdit, newText);
        var dbComments = await _postRepository.GetComments(editedPost.Id);
        
        List<CommentResult> comments = new();
        
        foreach (var item in dbComments)
        {
            var replies = SortReplies(await _postRepository.GetReplies(item.Id));
            comments.Add(new CommentResult(
                item.Id,
                item.PostId,
                item.UserId,
                item.Text,
                item.Date,
                replies));
        }
        
        return new PostResult(
            editedPost.Id,
            editedPost.UserId,
            editedPost.Text,
            editedPost.Date,
            SortComments(comments),
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
        foreach (var savedPost in posts)
        {
            var post = await _postRepository.GetPostById(savedPost.PostId);
            if (post is null)
                continue;
            
            var dbComments = await _postRepository.GetComments(post.Id);
            List<CommentResult> comments = new();
            foreach (var comment in dbComments)
            {
                var replies = SortReplies(await _postRepository.GetReplies(comment.Id));
                comments.Add(new CommentResult(
                    comment.Id,
                    comment.PostId,
                    comment.UserId,
                    comment.Text,
                    comment.Date,
                    replies));
            }
            savedPosts.Add(new PostResult(
                post.Id,
                post.UserId,
                post.Text,
                post.Date,
                SortComments(comments),
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
        var dbComments = await _postRepository.GetComments(postId);
        List<CommentResult> comments = new();

        foreach (var comment in dbComments)
        {
            var replies = SortReplies(await _postRepository.GetReplies(comment.Id));
            comments.Add(new CommentResult(
                comment.Id,
                comment.PostId,
                comment.UserId,
                comment.Text,
                comment.Date,
                replies));
        }

        return new PostResult(
            postToSave.Id,
            postToSave.UserId,
            postToSave.Text,
            postToSave.Date,
            SortComments(comments),
            postToSave.Likes);
    }

    public async Task<ErrorOr<string>> UnSavePost(string token, int postId)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var post = await _postRepository.GetPostById(postId);
        if (post is null)
            return Errors.Posts.PostNotFound;

        var postToUnSave = await _postRepository.GetSavedPostById(postId);
        if (postToUnSave is null)
            return Errors.Posts.SavedPostNotfound;

        await _postRepository.UnSavePost(postToUnSave);
        return Correct.Post.PostUnSaved;
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
        var dbComments = await _postRepository.GetComments(likedPost.Id);
        List<CommentResult> comments = new();
        
        foreach (var item in dbComments)
        {
            var replies = SortReplies(await _postRepository.GetReplies(item.Id));
            comments.Add(new CommentResult(
                item.Id,
                item.PostId,
                item.UserId,
                item.Text,
                item.Date,
                replies));
        }
        return new PostResult(
            likedPost.Id,
            likedPost.UserId,
            likedPost.Text,
            likedPost.Date,
            SortComments(comments),
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
        var dbComments = await _postRepository.GetComments(unLikedPost.Id);
        List<CommentResult> comments = new();
        
        foreach (var item in dbComments)
        {
            var replies = SortReplies(await _postRepository.GetReplies(item.Id));
            comments.Add(new CommentResult(
                item.Id,
                item.PostId,
                item.UserId,
                item.Text,
                item.Date,
                replies));
        }
        return new PostResult(
            unLikedPost.Id,
            unLikedPost.UserId,
            unLikedPost.Text,
            unLikedPost.Date,
            SortComments(comments),
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
            if (post is null)
                continue;
            
            var dbComments = await _postRepository.GetComments(post.Id);
            List<CommentResult> comments = new();
        
            foreach (var item in dbComments)
            {
                var replies = SortReplies(await _postRepository.GetReplies(item.Id));
                comments.Add(new CommentResult(
                    item.Id,
                    item.PostId,
                    item.UserId,
                    item.Text,
                    item.Date,
                    replies));
            }
            likedPosts.Add(new PostResult(
                post.Id,
                post.UserId,
                post.Text,
                post.Date,
                SortComments(comments),
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
            if (user is null)
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
            Date = DateTime.UtcNow.ToString("dd MMMM yyyy, HH:mm", CultureInfo.CreateSpecificCulture("en-US")),
            Text = text,
            UserId = _jwtTokenGenerator.ReadToken(token)
        });
        
        var dbComments = await _postRepository.GetComments(post.Id);
        List<CommentResult> comments = new();
        
        foreach (var item in dbComments)
        {
            var replies = SortReplies(await _postRepository.GetReplies(item.Id));
            comments.Add(new CommentResult(
                item.Id,
                item.PostId,
                item.UserId,
                item.Text,
                item.Date,
                replies));
        }
        return new PostResult(
            post.Id,
            post.UserId,
            post.Text,
            post.Date,
            SortComments(comments),
            post.Likes);
    }

    public async Task<ErrorOr<string>> RemoveComment(string token, int commentId)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;
        
        var commentToRemove = await _postRepository.GetCommentById(commentId);
        
        if (commentToRemove is null)
            return Errors.Posts.PostNotFound;

        if (_jwtTokenGenerator.ReadToken(token) != commentToRemove.UserId)
            return Errors.Authentication.WrongToken;
        
        await _postRepository.RemoveComment(commentToRemove);

        return Correct.Post.CommentRemoved;
    }

    public async Task<ErrorOr<PostResult>> Reply(string token, int userId, int commentId, string text, string date)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        if (await _postRepository.GetCommentById(commentId) is null)
            return Errors.Posts.CommentNotFound;

        if (await _userRepository.GetUserById(userId) is null)
            return Errors.Posts.CommentNotFound;

        await _postRepository.ReplyComment(new Reply
        {
            UserId = userId,
            ReplierId = _jwtTokenGenerator.ReadToken(token),
            CommentId = commentId,
            Data = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
            Text = text
        });
        var comment = await _postRepository.GetCommentById(commentId);
        var post = await _postRepository.GetPostById(comment!.PostId);
        
        var dbComments = await _postRepository.GetComments(post!.Id);
        List<CommentResult> comments = new();
        
        foreach (var item in dbComments)
        {
            var replies = SortReplies(await _postRepository.GetReplies(item.Id));
            comments.Add(new CommentResult(
                item.Id,
                item.PostId,
                item.UserId,
                item.Text,
                item.Date,
                replies));
        }
        return new PostResult(
            post.Id,
            post.UserId,
            post.Text,
            post.Date,
            SortComments(comments),
            post.Likes);
        
    }
    
    public async Task<ErrorOr<string>> RemoveReply(string token, int replyId)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;
        
        var replyToRemove = await _postRepository.GetReplyById(replyId);
        
        if (replyToRemove is null)
            return Errors.Posts.PostNotFound;

        if (_jwtTokenGenerator.ReadToken(token) != replyToRemove.UserId)
            return Errors.Authentication.WrongToken;
        
        await _postRepository.RemoveReply(replyToRemove);

        return Correct.Post.ReplyRemoved;
    }

    private List<Reply> SortReplies(List<Reply> replies)
        => replies
            .Select(r => {r.Data = DateTime.UtcNow
                .ToString("dd MMMM yyyy, HH:mm", CultureInfo.CreateSpecificCulture("en-US")); return r;})
            .OrderBy(r => r.Data)
            .ToList();

    private List<CommentResult> SortComments(List<CommentResult> comments)
        => comments.OrderBy(c => c.Date).ToList();

}