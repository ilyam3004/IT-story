namespace Contracts.Authentication;

public class RegisterRequest
{
    public string username { get; set; } = null!;
    public string email { get; set; } = null!;
    public string password { get; set; } = null!;
    public string confirmPassword { get; set; } = null!;
    public string firstName { get; set; } = null!;
    public string lastName { get; set; } = null!;
    public string status { get; set; } = null!;
}