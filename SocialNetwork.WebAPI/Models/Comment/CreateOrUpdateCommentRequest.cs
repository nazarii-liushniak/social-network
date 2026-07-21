namespace SocialNetwork.WebAPI.Models.Comment;

public record CreateOrUpdateCommentRequest
{
    public required string Content { get; set; }
}