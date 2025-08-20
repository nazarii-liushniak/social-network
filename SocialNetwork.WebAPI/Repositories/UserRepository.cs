using Microsoft.EntityFrameworkCore;
using SocialNetwork.WebAPI.Data;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Interfaces.Repositories;

namespace SocialNetwork.WebAPI.Repositories;

public class UserRepository(SocialNetworkDbContext context) : IUserRepository
{
    private readonly SocialNetworkDbContext _context = context;

    public async Task<User> AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        
        await _context.SaveChangesAsync();
        
        return user;
    }

    public async Task<User?> GetUserAsync(Guid userId)
    {
        var user =  await _context.Users.FindAsync(userId);
        
        return user;
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        var users = await _context.Users.ToListAsync();
        
        return users;
    }

    public async Task<IEnumerable<User>> GetFollowersAsync(Guid userId)
    {
        var users = await _context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Followers)
            .Select(f => f.Follower)
            .ToListAsync();
        
        return users;
    }

    public async Task<IEnumerable<User>> GetFollowingsAsync(Guid userId)
    {
        var users = await _context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Followings)
            .Select(f => f.Followee)
            .ToListAsync();
        
        return users;
    }

    public async Task<User?> UpdateUserAsync(User user)
    {
        var dbUser = await _context.Users.FindAsync(user.Id);
        
        if (dbUser == null)
            return null;
        
        user.Id = dbUser.Id;
        user.CreatedAt = dbUser.CreatedAt;
        
        _context.Users.Update(user);
        
        await _context.SaveChangesAsync();
        
        return user;
    }

    public async Task<User?> DeleteUserAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        
        if (user == null)
            return null;
        
        _context.Users.Remove(user);
        
        await _context.SaveChangesAsync();
        
        return user;
    }
}