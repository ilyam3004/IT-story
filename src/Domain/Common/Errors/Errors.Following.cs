using ErrorOr;

namespace Domain.Common.Errors;

public partial class Errors
{
    public class Following
    {
        public static Error UserToFollowNotFound => Error.NotFound(
            "Following.UserNotFound",
            "User would you want to follow not found.");
        
        public static Error UserToUnFollowNotFound => Error.NotFound(
            "Following.UserNotFound",
            "User would you want to unfollow not found.");
    }
}