using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Models.Comment;

public record CommentResponse
{
    public required Guid Id { get; set; }
    public required Author Author { get; set; }
    public required string Content { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
}