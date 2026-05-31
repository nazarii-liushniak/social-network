using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SocialNetwork.WebAPI.Converters;

public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
{
    public UtcDateTimeConverter() 
        : base(
            v => v,                                        // To DB (keeps UtcNow intact)
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc) // From DB (stamps it as Utc)
        )
    {
    }
}