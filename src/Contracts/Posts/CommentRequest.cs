namespace Contracts.Posts;
public record CommentRequest(
    int PostId,
    string Text);