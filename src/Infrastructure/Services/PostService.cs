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
            var dbComments = await _postRepository.GetComments(post.Post_id);
            List<CommentResponse> comments = new();
            foreach (var item in dbComments)
            {
                var dbReplies = SortReplies(await _postRepository.GetReplies(item.Comment_id));
                var replies = new List<ReplyResponse>();

                foreach (var dbReply in dbReplies)
                    replies.Add(new ReplyResponse(
                        dbReply.Reply_id,
                        dbReply.Comment_id,
                        (await _userRepository.GetUserById(dbReply.User_id))!,
                        (await _userRepository.GetUserById(dbReply.Replier_id))!,
                        dbReply.Text,
                        dbReply.Date));

                comments.Add(new CommentResponse(
                    item.Comment_id,
                    item.Post_id,
                    item.User_id,
                    item.Text,
                    item.Date,
                    replies));
            }

            userPosts.Add(new PostResult(
                post.Post_id,
                post.User_id,
                post.Text,
                post.Date,
                SortComments(comments),
                post.Likes_count));
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
            User_id = _jwtTokenGenerator.ReadToken(token),
            Text = text,
            Date = DateTime.UtcNow,
            Likes_count = 0
        };
        var post = await _postRepository.AddPost(postToAdd);
        var dbComments = await _postRepository.GetComments(post.Post_id);
        List<CommentResponse> comments = new();

        foreach (var item in dbComments)
        {
            var dbReplies = SortReplies(await _postRepository.GetReplies(item.Comment_id));
            var replies = new List<ReplyResponse>();

            foreach (var dbReply in dbReplies)
                replies.Add(new ReplyResponse(
                    dbReply.Reply_id,
                    dbReply.Comment_id,
                    (await _userRepository.GetUserById(dbReply.User_id))!,
                    (await _userRepository.GetUserById(dbReply.Replier_id))!,
                    dbReply.Text,
                    dbReply.Date));

            comments.Add(new CommentResponse(
                item.Comment_id,
                item.Post_id,
                item.User_id,
                item.Text,
                item.Date,
                replies));
        }
        return new PostResult(
            post.Post_id,
            post.User_id,
            post.Text,
            post.Date,
            SortComments(comments),
            0);
    }

    public async Task<ErrorOr<Message>> RemovePost(string token, int postId)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var postToRemove = await _postRepository.GetPostById(postId);

        if (postToRemove is null)
            return Errors.Posts.PostNotFound;

        if (_jwtTokenGenerator.ReadToken(token) != postToRemove.User_id)
            return Errors.Authentication.WrongToken;

        await _postRepository.RemovePost(postToRemove);

        return new Message(Correct.Post.PostRemoved);
    }

    public async Task<ErrorOr<PostResult>> EditPost(int postId, string newText, string token)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var postToEdit = await _postRepository.GetPostById(postId);
        if (postToEdit is null)
            return Errors.Posts.PostNotFound;

        if (_jwtTokenGenerator.ReadToken(token) != postToEdit.User_id)
            return Errors.Authentication.WrongToken;

        var editedPost = await _postRepository.EditPost(postToEdit, newText);
        var dbComments = await _postRepository.GetComments(editedPost.Post_id);

        List<CommentResponse> comments = new();

        foreach (var item in dbComments)
        {
            var dbReplies = SortReplies(await _postRepository.GetReplies(item.Comment_id));
            var replies = new List<ReplyResponse>();

            foreach (var dbReply in dbReplies)
                replies.Add(new ReplyResponse(
                    dbReply.Reply_id,
                    dbReply.Comment_id,
                    (await _userRepository.GetUserById(dbReply.User_id))!,
                    (await _userRepository.GetUserById(dbReply.Replier_id))!,
                    dbReply.Text,
                    dbReply.Date));

            comments.Add(new CommentResponse(
                item.Comment_id,
                item.Post_id,
                item.User_id,
                item.Text,
                item.Date,
                replies));
        }
        return new PostResult(
            editedPost.Post_id,
            editedPost.User_id,
            editedPost.Text,
            editedPost.Date,
            SortComments(comments),
            editedPost.Likes_count);
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
            var post = await _postRepository.GetPostById(savedPost.Post_id);
            if (post is null)
                continue;

            var dbComments = await _postRepository.GetComments(post.Post_id);
            List<CommentResponse> comments = new();
            foreach (var item in dbComments)
            {
                var dbReplies = SortReplies(await _postRepository.GetReplies(item.Comment_id));
                var replies = new List<ReplyResponse>();

                foreach (var dbReply in dbReplies)
                    replies.Add(new ReplyResponse(
                        dbReply.Reply_id,
                        dbReply.Comment_id,
                        (await _userRepository.GetUserById(dbReply.User_id))!,
                        (await _userRepository.GetUserById(dbReply.Replier_id))!,
                        dbReply.Text,
                        dbReply.Date));

                comments.Add(new CommentResponse(
                    item.Comment_id,
                    item.Post_id,
                    item.User_id,
                    item.Text,
                    item.Date,
                    replies));
            }
            savedPosts.Add(new PostResult(
                post.Post_id,
                post.User_id,
                post.Text,
                post.Date,
                SortComments(comments),
                post.Likes_count));
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


        await _postRepository.SavePost(new FavouritePost
        {
            Post_id = postToSave.Post_id,
            User_id = _jwtTokenGenerator.ReadToken(token)
        });
        var dbComments = await _postRepository.GetComments(postId);
        List<CommentResponse> comments = new();

        foreach (var item in dbComments)
        {
            var dbReplies = SortReplies(await _postRepository.GetReplies(item.Comment_id));
            var replies = new List<ReplyResponse>();

            foreach (var dbReply in dbReplies)
                replies.Add(new ReplyResponse(
                    dbReply.Reply_id,
                    dbReply.Comment_id,
                    (await _userRepository.GetUserById(dbReply.User_id))!,
                    (await _userRepository.GetUserById(dbReply.Replier_id))!,
                    dbReply.Text,
                    dbReply.Date));

            comments.Add(new CommentResponse(
                item.Comment_id,
                item.Post_id,
                item.User_id,
                item.Text,
                item.Date,
                replies));
        }
        return new PostResult(
            postToSave.Post_id,
            postToSave.User_id,
            postToSave.Text,
            postToSave.Date,
            SortComments(comments),
            postToSave.Likes_count);
    }

    public async Task<ErrorOr<Message>> UnSavePost(string token, int postId)
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
        return new Message(Correct.Post.PostUnSaved);
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

        var postLikes = await _postRepository.GetPostLikes(postId);

        var like = postLikes.FirstOrDefault(l => l.User_id == _jwtTokenGenerator.ReadToken(token));
        if (like is not null)
            return Errors.Posts.AlreadyLiked;


        var likedPost = await _postRepository.LikePost(post, _jwtTokenGenerator.ReadToken(token));
        var dbComments = await _postRepository.GetComments(likedPost.Post_id);
        List<CommentResponse> comments = new();

        foreach (var item in dbComments)
        {
            var dbReplies = SortReplies(await _postRepository.GetReplies(item.Comment_id));
            var replies = new List<ReplyResponse>();

            foreach (var dbReply in dbReplies)
                replies.Add(new ReplyResponse(
                    dbReply.Reply_id,
                    dbReply.Comment_id,
                    (await _userRepository.GetUserById(dbReply.User_id))!,
                    (await _userRepository.GetUserById(dbReply.Replier_id))!,
                    dbReply.Text,
                    dbReply.Date));

            comments.Add(new CommentResponse(
                item.Comment_id,
                item.Post_id,
                item.User_id,
                item.Text,
                item.Date,
                replies));
        }
        return new PostResult(
            post.Post_id,
            post.User_id,
            post.Text,
            post.Date,
            SortComments(comments),
            post.Likes_count);
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

        var postLikes = await _postRepository.GetPostLikes(postId);
        var like = postLikes.FirstOrDefault(l => l.User_id == _jwtTokenGenerator.ReadToken(token));
        if (like is null)
            return Errors.Posts.PostWasNotLiked;

        var unLikedPost = await _postRepository.UnLikePost(post, _jwtTokenGenerator.ReadToken(token));
        var dbComments = await _postRepository.GetComments(unLikedPost.Post_id);
        List<CommentResponse> comments = new();

        foreach (var item in dbComments)
        {
            var dbReplies = SortReplies(await _postRepository.GetReplies(item.Comment_id));
            var replies = new List<ReplyResponse>();

            foreach (var dbReply in dbReplies)
                replies.Add(new ReplyResponse(
                    dbReply.Reply_id,
                    dbReply.Comment_id,
                    (await _userRepository.GetUserById(dbReply.User_id))!,
                    (await _userRepository.GetUserById(dbReply.Replier_id))!,
                    dbReply.Text,
                    dbReply.Date));

            comments.Add(new CommentResponse(
                item.Comment_id,
                item.Post_id,
                item.User_id,
                item.Text,
                item.Date,
                replies));
        }
        return new PostResult(
            post.Post_id,
            post.User_id,
            post.Text,
            post.Date,
            SortComments(comments),
            post.Likes_count);
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
            var post = await _postRepository.GetPostById(like.Post_id);
            if (post is null)
                continue;

            var dbComments = await _postRepository.GetComments(post.Post_id);
            List<CommentResponse> comments = new();

            foreach (var item in dbComments)
            {
                var dbReplies = SortReplies(await _postRepository.GetReplies(item.Comment_id));
                var replies = new List<ReplyResponse>();

                foreach (var dbReply in dbReplies)
                    replies.Add(new ReplyResponse(
                        dbReply.Reply_id,
                        dbReply.Comment_id,
                        (await _userRepository.GetUserById(dbReply.User_id))!,
                        (await _userRepository.GetUserById(dbReply.Replier_id))!,
                        dbReply.Text,
                        dbReply.Date));

                comments.Add(new CommentResponse(
                    item.Comment_id,
                    item.Post_id,
                    item.User_id,
                    item.Text,
                    item.Date,
                    replies));
            }
            likedPosts.Add(new PostResult(
                post.Post_id,
                post.User_id,
                post.Text,
                post.Date,
                SortComments(comments),
                post.Likes_count));
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

    public async Task<ErrorOr<PostResult>> CommentPost(string token, int postId, string text)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

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

        var dbComments = await _postRepository.GetComments(post.Post_id);
        List<CommentResponse> comments = new();

        foreach (var item in dbComments)
        {
            var dbReplies = SortReplies(await _postRepository.GetReplies(item.Comment_id));
            var replies = new List<ReplyResponse>();

            foreach (var dbReply in dbReplies)
                replies.Add(new ReplyResponse(
                    dbReply.Reply_id,
                    dbReply.Comment_id,
                    (await _userRepository.GetUserById(dbReply.User_id))!,
                    (await _userRepository.GetUserById(dbReply.Replier_id))!,
                    dbReply.Text,
                    dbReply.Date));

            comments.Add(new CommentResponse(
                item.Comment_id,
                item.Post_id,
                item.User_id,
                item.Text,
                item.Date,
                replies));
        }
        return new PostResult(
            post.Post_id,
            post.User_id,
            post.Text,
            post.Date,
            SortComments(comments),
            post.Likes_count);
    }

    public async Task<ErrorOr<Message>> RemoveComment(string token, int commentId)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var commentToRemove = await _postRepository.GetCommentById(commentId);

        if (commentToRemove is null)
            return Errors.Posts.CommentNotFound;

        if (_jwtTokenGenerator.ReadToken(token) != commentToRemove.User_id)
            return Errors.Authentication.WrongToken;

        await _postRepository.RemoveComment(commentToRemove);

        return new Message(Correct.Post.CommentRemoved);
    }

    public async Task<ErrorOr<PostResult>> Reply(string token, int userId, int commentId, string text)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

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
        var post = await _postRepository.GetPostById(comment!.Post_id);

        var dbComments = await _postRepository.GetComments(post!.Post_id);
        List<CommentResponse> comments = new();

        foreach (var item in dbComments)
        {
            var dbReplies = SortReplies(await _postRepository.GetReplies(item.Comment_id));
            var replies = new List<ReplyResponse>();

            foreach (var dbReply in dbReplies)
                replies.Add(new ReplyResponse(
                    dbReply.Reply_id,
                    dbReply.Comment_id,
                    (await _userRepository.GetUserById(dbReply.User_id))!,
                    (await _userRepository.GetUserById(dbReply.Replier_id))!,
                    dbReply.Text,
                    dbReply.Date));

            comments.Add(new CommentResponse(
                item.Comment_id,
                item.Post_id,
                item.User_id,
                item.Text,
                item.Date,
                replies));
        }
        return new PostResult(
            post.Post_id,
            post.User_id,
            post.Text,
            post.Date,
            SortComments(comments),
            post.Likes_count);
    }

    public async Task<ErrorOr<Message>> RemoveReply(string token, int replyId)
    {
        if (token == String.Empty)
            return Errors.Authentication.TokenNotFound;

        if (!_jwtTokenGenerator.CanReadToken(token))
            return Errors.Authentication.WrongToken;

        var replyToRemove = await _postRepository.GetReplyById(replyId);

        if (replyToRemove is null)
            return Errors.Posts.PostNotFound;

        if (_jwtTokenGenerator.ReadToken(token) != replyToRemove.User_id)
            return Errors.Authentication.WrongToken;

        await _postRepository.RemoveReply(replyToRemove);

        return new Message(Correct.Post.ReplyRemoved);
    }

    private List<PostReply> SortReplies(List<PostReply> replies)
        => replies
            .OrderBy(r => r.Date)
            .ToList();

    private List<CommentResponse> SortComments(List<CommentResponse> comments)
        => comments.OrderBy(c => c.Date).ToList();
}