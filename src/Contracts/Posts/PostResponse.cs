namespace Contracts.Posts;

public record PostResponse(
    int Id,
    string Email,
    string Text,
    string Date);