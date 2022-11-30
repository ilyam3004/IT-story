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

    public async Task<ErrorOr<List<PostResponse>>> GetPosts(string token)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var posts = await _postRepository.GetPostsByUserId(_jwtTokenGenerator.ReadToken(token));
        if (posts.Count == 0)
            return Errors.Posts.PostsNotFound;

        List<PostResponse> userPosts = new();
        foreach (var post in posts)
            userPosts.Add(await MapPostResponse(post));

        return userPosts;
    }

    public async Task<ErrorOr<PostResponse>> AddPost(string text, string token)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var postToAdd = new Post()
        {
            User_id = _jwtTokenGenerator.ReadToken(token),
            Text = text,
            Date = DateTime.UtcNow,
            Likes_count = 0
        };
        var post = await _postRepository.AddPost(postToAdd);
        
        return await MapPostResponse(post);
    }

    public async Task<ErrorOr<MessageResponse>> RemovePost(string token, int postId)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var postToRemove = await _postRepository.GetPostById(postId);

        if (postToRemove is null)
            return Errors.Posts.PostNotFound;

        if (_jwtTokenGenerator.ReadToken(token) != postToRemove.User_id)
            return Errors.Authentication.WrongToken;

        await _postRepository.RemovePost(postToRemove);

        return new MessageResponse(Correct.Post.PostRemoved);
    }

    public async Task<ErrorOr<PostResponse>> EditPost(int postId, string newText, string token)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var postToEdit = await _postRepository.GetPostById(postId);
        if (postToEdit is null)
            return Errors.Posts.PostNotFound;

        if (_jwtTokenGenerator.ReadToken(token) != postToEdit.User_id)
            return Errors.Authentication.WrongToken;

        var editedPost = await _postRepository.EditPost(postToEdit, newText);
        var dbComments = await _postRepository.GetComments(editedPost.Post_id);

        return await MapPostResponse(editedPost);
    }

    public async Task<ErrorOr<List<PostResponse>>> GetSavedPosts(string token)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var posts = await _postRepository.GetSavedPosts(_jwtTokenGenerator.ReadToken(token));

        if (posts.Count == 0)
            return Errors.Posts.SavedPostsNotfound;

        List<PostResponse> savedPosts = new();
        foreach (var savedPost in posts)
        {
            var post = await _postRepository.GetPostById(savedPost.Post_id);
            if (post is null)
                continue;

            savedPosts.Add(await MapPostResponse(post));
        }

        return savedPosts;
    }

    public async Task<ErrorOr<MessageResponse>> SavePost(string token, int postId)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var postToSave = await _postRepository.GetPostById(postId);

        if (postToSave is null)
            return Errors.Posts.PostNotFound;

        FavouritePost? savedPost = await _postRepository.GetSavedPostById(postId);

        if (savedPost is not null)
            return Errors.Posts.PostAlreadySaved;

        await _postRepository.SavePost(new FavouritePost
        {
            Post_id = postToSave.Post_id,
            User_id = _jwtTokenGenerator.ReadToken(token)
        });

        return new MessageResponse(Correct.Post.PostSaved);
    }

    public async Task<ErrorOr<MessageResponse>> UnSavePost(string token, int postId)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var post = await _postRepository.GetPostById(postId);
        if (post is null)
            return Errors.Posts.PostNotFound;

        var postToUnSave = await _postRepository.GetSavedPostById(postId);
        if (postToUnSave is null)
            return Errors.Posts.SavedPostNotfound;

        await _postRepository.UnSavePost(postToUnSave);

        return new MessageResponse(Correct.Post.PostUnSaved);
    }

    public async Task<ErrorOr<PostResponse>> LikePost(string token, int postId)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var post = await _postRepository.GetPostById(postId);
        if (post is null)
            return Errors.Posts.PostNotFound;

        var postLikes = await _postRepository.GetPostLikes(postId);

        var like = postLikes.FirstOrDefault(l => l.User_id == _jwtTokenGenerator.ReadToken(token));
        if (like is not null)
            return Errors.Posts.AlreadyLiked;

        var likedPost = await _postRepository.LikePost(post, _jwtTokenGenerator.ReadToken(token));

        return await MapPostResponse(likedPost);
    }

    public async Task<ErrorOr<PostResponse>> UnLikePost(string token, int postId)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var post = await _postRepository.GetPostById(postId);
        if (post is null)
            return Errors.Posts.PostNotFound;

        var postLikes = await _postRepository.GetPostLikes(postId);
        var like = postLikes.FirstOrDefault(l => l.User_id == _jwtTokenGenerator.ReadToken(token));
        if (like is null)
            return Errors.Posts.PostWasNotLiked;

        var unLikedPost = await _postRepository.UnLikePost(post, _jwtTokenGenerator.ReadToken(token));

        return await MapPostResponse(unLikedPost);
    }

    public async Task<ErrorOr<List<PostResponse>>> GetLikedPosts(string token)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var likes = await _postRepository.LikedPosts(_jwtTokenGenerator.ReadToken(token));
        if (likes.Count == 0)
            return Errors.Posts.DontLikeAnyPost;

        List<PostResponse> likedPosts = new();
        foreach (var like in likes)
        {
            var post = await _postRepository.GetPostById(like.Post_id);
            if (post is null)
                continue;

            likedPosts.Add(await MapPostResponse(post));
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
            var user = await _userRepository.GetUserById(like.User_id);
            if (user is null)
                continue;
            users.Add(new UserLikedPost(
                user.User_id,
                user.Username,
                user.FirstName,
                user.LastName));
        }

        return users;
    }

    public async Task<ErrorOr<PostResponse>> CommentPost(string token, int postId, string text)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var post = await _postRepository.GetPostById(postId);
        if (post is null)
            return Errors.Posts.PostNotFound;

        await _postRepository.CreateComment(new PostComment
        {
            Post_id = post.Post_id,
            Date = DateTime.UtcNow,
            Text = text,
            User_id = _jwtTokenGenerator.ReadToken(token)
        });

        return await MapPostResponse(post);
    }

    public async Task<ErrorOr<MessageResponse>> RemoveComment(string token, int commentId)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var commentToRemove = await _postRepository.GetCommentById(commentId);

        if (commentToRemove is null)
            return Errors.Posts.CommentNotFound;

        if (_jwtTokenGenerator.ReadToken(token) != commentToRemove.User_id)
            return Errors.Authentication.WrongToken;

        await _postRepository.RemoveComment(commentToRemove);

        return new MessageResponse(Correct.Post.CommentRemoved);
    }

    public async Task<ErrorOr<PostResponse>> Reply(string token, int userId, int commentId, string text)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var comment = await _postRepository.GetCommentById(commentId);
        
        if (comment is null)
            return Errors.Posts.CommentNotFound;

        if (await _userRepository.GetUserById(userId) is null)
            return Errors.Posts.CommentNotFound;

        await _postRepository.ReplyComment(new PostReply
        {
            User_id = userId,
            Replier_id = _jwtTokenGenerator.ReadToken(token),
            Comment_id = commentId,
            Date = DateTime.UtcNow,
            Text = text
        });

        var post = await _postRepository.GetPostById(comment.Post_id);

        return await MapPostResponse(post!);
    }

    public async Task<ErrorOr<MessageResponse>> RemoveReply(string token, int replyId)
    {
        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var replyToRemove = await _postRepository.GetReplyById(replyId);

        if (replyToRemove is null)
            return Errors.Posts.PostNotFound;

        if (_jwtTokenGenerator.ReadToken(token) != replyToRemove.User_id)
            return Errors.Authentication.WrongToken;

        await _postRepository.RemoveReply(replyToRemove);

        return new MessageResponse(Correct.Post.ReplyRemoved);
    }

    private async Task<PostResponse> MapPostResponse(Post post)
    {
        List<PostCommentResponse> comments = await MapPostCommentResponse(
            await _postRepository.GetComments(post.Post_id));

        return new PostResponse(
            post.Post_id,
            post.User_id,
            post.Text,
            post.Date,
            comments,
            post.Likes_count);
    }

    private async Task<List<PostCommentResponse>> MapPostCommentResponse(List<PostComment> dbPostComments)
    {
        List<PostCommentResponse> postComments = new();

        foreach (var dbComment in dbPostComments)
        {
            List<PostReplyResponse> postReplies = await MapPostReplyResponse(
                await _postRepository.GetReplies(dbComment.Comment_id));

            postComments.Add(new PostCommentResponse(
                dbComment.Comment_id,
                dbComment.Post_id,
                dbComment.User_id,
                dbComment.Text,
                dbComment.Date,
                postReplies));
        }

        return SortCommentsByDate(postComments);
    }

    private async Task<List<PostReplyResponse>> MapPostReplyResponse(List<PostReply> dbPostReplies)
    {
        List<PostReplyResponse> postReplies = new();

        foreach (var dbReply in dbPostReplies)
            postReplies.Add(new PostReplyResponse(
                dbReply.Reply_id,
                dbReply.Comment_id,
                (await _userRepository.GetUserById(dbReply.User_id))!,
                (await _userRepository.GetUserById(dbReply.Replier_id))!,
                dbReply.Text,
                dbReply.Date));

        return SortRepliesByDate(postReplies);
    }

    private List<PostReplyResponse> SortRepliesByDate(List<PostReplyResponse> replies)
        => replies
            .OrderBy(r => r.Date)
            .ToList();

    private List<PostCommentResponse> SortCommentsByDate(List<PostCommentResponse> comments)
        => comments
            .OrderBy(c => c.Date)
            .ToList();
}