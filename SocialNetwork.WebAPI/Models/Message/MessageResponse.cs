namespace SocialNetwork.WebAPI.Models.Message;

public record MessageResponse
{
    public required Guid Id { get; set; }
    public required MessageDirection Direction { get; set; }
    public required string Content { get; set; }
    public required DateTimeOffset SentAt { get; set; }
}