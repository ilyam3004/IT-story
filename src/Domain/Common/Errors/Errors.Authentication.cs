using ErrorOr;

namespace Domain.Common.Errors;

public static partial class Errors 
{
    public static class Authentication
    {
        public static Error InvalidPassword => Error.Validation(
            "Auth.InvalidPassword",
            "Password length must be less than 60 and more than 8");

        public static Error InvalidEmail => Error.Validation(
            "Auth.InvalidEmail",
            "Invalid email address");

        public static Error PasswordsDoNotMatch => Error.Validation(
            "Auth.PasswordsFoNotMatch",
            "Passwords do not match");
        
        public static Error DuplicateEmail => Error.Conflict(
            "Auth.DuplicateEmail", 
            "User with this email already exists");
        
        public static Error DuplicateUserName => Error.Conflict(
            "Auth.DuplicateUserName",
            "User with this username already exists");

        public static Error UserNotFound => Error.NotFound(
            "Auth.UserNotFound",
            "Account not found");
        
        public static Error WrongToken => Error.Conflict(
            "Post.WrongToken",
            "Wrong token");
        
        public static Error TokenNotFound => Error.NotFound(
            "Post.TokenNotFound",
            "Token not found");
    }
}