namespace SocialNetwork.WebAPI.Models.Post;

public class Posts
{
    public ICollection<PostElement> Items { get; set; } = [];
    public int TotalPosts { get; set; }
    public int Limit { get; set; }
}