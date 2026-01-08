using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Models.Comment;

public class Comment
{
    public Guid Id { get; set; }
    public required ShortProfile Author { get; set; }
    public required string Content { get; set; }
    public DateTime Timestamp { get; set; }
}