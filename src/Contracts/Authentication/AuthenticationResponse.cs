namespace Contracts.Authentication;

public class AuthenticationResponse
{
    public int id { get; set; }
    public string token { get; set; } = null!;
    public string username { get; set; } = null!;
    public string email { get; set; } = null!;
    public string firstName { get; set; } = null!;
    public string lastName { get; set; } = null!;
    public string status { get; set; } = null!;
}