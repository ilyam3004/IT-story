using ErrorOr;

namespace Domain.Common.Errors;

public partial class Errors
{
    public class Posts
    {
        public static Error PostsNotFound => Error.NotFound(
            "Post.PostsNotFound",
            "Share your first post.");
        
        public static Error PostNotFound => Error.NotFound(
            "Post.PostNotFound",
            "Post not found");
        
        public static Error TokenNotFound => Error.NotFound(
            "Post.TokenNotFound",
            "Token not found");

        public static Error WrongToken => Error.Conflict(
            "Post.WrongToken",
            "Wrong token");
    }
}