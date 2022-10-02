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
            "Nobody liked this post yet");
        
        public static Error AlreadyLiked => Error.Conflict(
            "Post.AlreadyLiked",
            "You already liked this post");

        public static Error CommentNotFound => Error.NotFound(
            "Post.CommentNotFound",
            "Comment not found");
        
        public static Error PostWasNotLiked => Error.Conflict(
            "Post.PostWasNotLiked",
            "This post wasn't liked by you.");
    }
    public class Articles
    {
        public static Error ArticlesNotFound => Error.NotFound(
            "Articles.NotFound",
            "Articles not found. Share your first article");
        
        public static Error ArticleAlreadyLiked => Error.Conflict(
            "Article.AlreadyLiked",
            "You already liked this article");
        
        public static Error DontLikeAnyArticle => Error.NotFound(
            "Article.YouDidn'tLikeAnyArticle",
            "You didn't like any article");

        public static Error LikesNotFound => Error.NotFound(
            "Article.LikesNotFound",
            "Nobody liked this article yet");

        public static Error CommentNotFound => Error.NotFound(
                "Article.CommentNotFound",
                "Comment not found");
        
        public static Error ArticleNotFound => Error.NotFound(
            "Article.NotFound",
            "Article not found");
        
        public static Error ReplyNotFound => Error.NotFound(
            "Reply.NotFound",
            "Reply not found");
    }
}