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
    }
}