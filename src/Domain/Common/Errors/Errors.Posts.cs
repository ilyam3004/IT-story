using ErrorOr;

namespace Domain.Common.Errors;

public partial class Errors
{
    public class Posts
    {
        public static Error PostsNotFound => Error.NotFound(
            "Post.PostsNotFound",
            "Share your first post");
        
        public static Error PostNotFound => Error.NotFound(
            "Post.PostNotFound",
            "Post not found");
        
        public static Error SavedPostsNotfound => Error.NotFound(
            "Post.SavedPostsNotFound",
            "Saved posts not found");
        public static Error SavedPostNotfound => Error.NotFound(
            "Post.SavedPostNotFound",
            "Saved post not found");
        
        public static Error PostAlreadySaved => Error.Conflict(
            "Post.PostAlreadySaved",
            "Post already saved");
        
        public static Error DontLikeAnyPost => Error.NotFound(
            "Post.YouDidn'tLikeAnyPost",
            "You didn't like any post");
        
        public static Error LikesNotFound => Error.NotFound(
            "Post.LikesNotFound",
            "Likes not found");

        public static Error CommentNotFound => Error.NotFound(
            "Post.CommentNotFound",
            "Comment not found");
    }
}