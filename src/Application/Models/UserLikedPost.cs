namespace Application.Models;

public record UserLikedPost(
    int Id,
    string Email,
    string Username,
    string FirstName,
    string LastName,
    string Status);