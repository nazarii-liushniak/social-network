using Microsoft.AspNetCore.JsonPatch;
using SocialNetwork.WebAPI.Models.Comment;
using SocialNetwork.WebAPI.Models.Post;
using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Interfaces.Services;

public interface IPostService
{
    public SinglePost Post(CreatePost post);
    public IEnumerable<FeedPost> GetFeed(Guid userId);
    public SinglePost GetPost(Guid postId);
    public SinglePost UpdatePost(JsonPatchDocument<CreatePost> post);
    public bool DeletePost(Guid postId);
    public bool LikePost(Guid postId);
    public bool UnlikePost(Guid postId);
    public IEnumerable<ShortProfile> GetUsersLikedPost(Guid userId);
    public Comment CreateComment(CreateComment comment);
    public IEnumerable<Comment> GetComments(Guid postId);
    public Comment UpdateComment(JsonPatchDocument<Comment> comment);
    public bool DeleteComment(Guid commentId);
}