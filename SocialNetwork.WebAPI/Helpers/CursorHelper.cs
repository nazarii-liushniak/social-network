using System.Text;

namespace SocialNetwork.WebAPI.Helpers;

public static class CursorHelper
{
    public static string GenerateCursor(DateTime timestamp, Guid id)
    {
        var timestampString = timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fff");
        var idString = id.ToString("N");
        
        var cursorString = new StringBuilder(timestampString)
            .Append('|')
            .Append(idString)
            .ToString();
        
        var cursorBytes = Encoding.UTF8.GetBytes(cursorString);

        return Convert.ToBase64String(cursorBytes);
    }
    
    public static string GenerateCursor(DateTime timestamp, Guid followerId, Guid followeeId)
    {
        var timestampString = timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fff");
        var followerIdString = followerId.ToString("N");
        var followeeIdString = followeeId.ToString("N");
        
        var cursorString = new StringBuilder(timestampString)
            .Append('|')
            .Append(followerIdString)
            .Append('|')
            .Append(followeeIdString)
            .ToString();
        
        var cursorBytes = Encoding.UTF8.GetBytes(cursorString);

        return Convert.ToBase64String(cursorBytes);
    }
    
    public static (DateTime, Guid) ParseCursor(string cursor)
    {
        var cursorBytes = Convert.FromBase64String(cursor);
        var cursorString = Encoding.UTF8.GetString(cursorBytes);
        var splitCursorString = cursorString.Split('|');

        var timestampString = splitCursorString[0];
        var idString = splitCursorString[1];
        
        var timestamp = DateTime.Parse(timestampString);
        var id = Guid.Parse(idString);
        
        return (timestamp, id);
    }
    
    public static (DateTime, Guid, Guid) ParseCursorForFollows(string cursor)
    {
        var cursorBytes = Convert.FromBase64String(cursor);
        var cursorString = Encoding.UTF8.GetString(cursorBytes);
        var splitCursorString = cursorString.Split('|');

        var timestampString = splitCursorString[0];
        var followerIdString = splitCursorString[1];
        var followeeIdString = splitCursorString[2];
        
        var timestamp = DateTime.Parse(timestampString);
        var followerId = Guid.Parse(followerIdString);
        var followeeId = Guid.Parse(followeeIdString);
        
        return (timestamp, followerId, followeeId);
    }
}