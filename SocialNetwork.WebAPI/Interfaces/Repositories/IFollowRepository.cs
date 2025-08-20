using SocialNetwork.WebAPI.Entities;

namespace SocialNetwork.WebAPI.Interfaces.Repositories;

public interface IFollowRepository
{
    Task<Follow> AddFollowAsync(Follow follow);
    Task<Follow?> DeleteFollowAsync(Guid followId);
}