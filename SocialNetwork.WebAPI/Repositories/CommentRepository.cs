using Microsoft.EntityFrameworkCore;
using SocialNetwork.WebAPI.Data;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Interfaces.Repositories;

namespace SocialNetwork.WebAPI.Repositories;

public class CommentRepository(SocialNetworkDbContext context) : ICommentRepository
{
    private readonly SocialNetworkDbContext _context = context;

    public async Task<Comment> AddCommentAsync(Comment comment)
    {
        await _context.Comments.AddAsync(comment);
        
        await _context.SaveChangesAsync();

        return comment;
    }

    public async Task<Comment?> GetCommentAsync(Guid id)
    {
        var comment = await _context.Comments.FindAsync(id);
        
        return comment;
    }

    public async Task<IEnumerable<Comment>> GetCommentsAsync(Guid postId)
    {
        var comments = await _context.Comments
            .Where(c => c.PostId == postId).ToListAsync();
        
        return comments;
    }

    public async Task<Comment?> UpdateCommentAsync(Comment comment)
    {
        var dbComment = await _context.Comments.FindAsync(comment.Id);

        if (dbComment == null)
            return null;
        
        comment.Id = dbComment.Id;
        comment.PostId = dbComment.PostId;
        comment.UserId = dbComment.UserId;
        comment.CreatedAt = dbComment.CreatedAt;
        
        _context.Comments.Update(comment);
        
        await _context.SaveChangesAsync();

        return comment;
    }

    public async Task<Comment?> DeleteCommentAsync(Guid id)
    {
        var comment = await _context.Comments.FindAsync(id);
        
        if (comment == null)
            return null;
        
        _context.Comments.Remove(comment);
        
        await _context.SaveChangesAsync();
        
        return comment;
    }
}