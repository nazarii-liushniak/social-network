using FluentResults;

namespace SocialNetwork.WebAPI.Errors;

public class ForbiddenError(string message) : Error(message)
{
}