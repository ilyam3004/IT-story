using ErrorOr;

namespace Domain.Common.Errors;

public static partial class Errors 
{
    public static class Authentication
    {
        public static Error InvallidCredentials => Error.Validation(
            "Auth.InvalidCred", 
            "Invalid credentials");
    }
}