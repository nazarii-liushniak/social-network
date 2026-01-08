namespace SocialNetwork.WebAPI.Models.Comment;

public class Comments
{
    public ICollection<Comment> Items { get; set; } = [];
    public int TotalComments { get; set; }
    public int Limit { get; set; }
}