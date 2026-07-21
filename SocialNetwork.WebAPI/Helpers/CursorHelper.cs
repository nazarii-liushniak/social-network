using System.Globalization;
using System.Text;

namespace SocialNetwork.WebAPI.Helpers;

public static class CursorHelper
{
    public static string GenerateCursor(DateTimeOffset timestamp, Guid id)
    {
        var timestampString = timestamp.ToString("O");
        var idString = id.ToString("N");
        
        var cursorString = new StringBuilder(timestampString)
            .Append('|')
            .Append(idString)
            .ToString();
        
        var cursorBytes = Encoding.UTF8.GetBytes(cursorString);

        return Convert.ToBase64String(cursorBytes);
    }
    
    public static bool TryParseCursor(string? cursor, out DateTimeOffset? timestamp, out Guid? id)
    {
        timestamp = null;
        id = null;

        if (string.IsNullOrWhiteSpace(cursor))
            return true;

        try
        {
            var cursorBytes = Convert.FromBase64String(cursor);
            var cursorString = Encoding.UTF8.GetString(cursorBytes);

            var splitCursorString = cursorString.Split('|');
            if (splitCursorString.Length != 2)
                return false;

            var timestampString = splitCursorString[0];
            var idString = splitCursorString[1];
            
            var isTimestampParsed = DateTimeOffset.TryParse(timestampString, CultureInfo.InvariantCulture, out var parsedTimestamp);
            var isIdParsed = Guid.TryParse(idString, CultureInfo.InvariantCulture, out var parsedId);

            if (!isTimestampParsed || !isIdParsed)
                return false;
            
            timestamp = parsedTimestamp;
            id = parsedId;
            
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}