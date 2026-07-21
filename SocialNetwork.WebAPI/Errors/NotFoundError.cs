using FluentResults;

namespace SocialNetwork.WebAPI.Errors;

public class NotFoundError(string message) : Error(message)
{
}