using Domain.Entities;

namespace Application.Models;

public record CommentResult(
    int Id,
    int PostId,
    int UserId,
    string Text,
    string Date,
    List<Reply> Replies);
