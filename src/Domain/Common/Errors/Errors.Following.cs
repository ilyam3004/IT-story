using ErrorOr;
namespace Domain.Common.Errors;

public partial class Errors
{
    public class Following
    {
        public static Error UserToFollowNotFound => Error.NotFound(
            "Following.UserNotFound",
            "User would you want to follow not found");
        
        public static Error UserToUnFollowNotFound => Error.NotFound(
            "Following.UserNotFound",
            "User would you want to unfollow not found");
        
        public static Error FollowersNotFound => Error.NotFound(
            "Following.FollowersNotFound",
            "Followers not found");
        public static Error FollowingsNotFound => Error.NotFound(
            "Following.FollowingsNotFound",
            "Followings not found");
    }
}