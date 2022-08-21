namespace Contracts.Posts;

public record ReplyRequest(
    int CommentId,
    int UserId,
    string Text);