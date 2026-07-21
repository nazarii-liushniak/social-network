namespace SocialNetwork.WebAPI.Models;

public record PagedResponse<T>(IReadOnlyList<T> Items, string? NextCursor);