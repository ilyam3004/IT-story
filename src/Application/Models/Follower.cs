namespace Application.Models;

public record Follower(
    int Id,
    string Email,
    string Username,
    string FirstName,
    string LastName,
    string Status);