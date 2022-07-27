namespace Contracts.Posts;

public record PostRequest(
    string Email,
    string Text,
    string Date);