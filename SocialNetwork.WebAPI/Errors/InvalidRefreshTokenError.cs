using FluentResults;

namespace SocialNetwork.WebAPI.Errors;

public class InvalidRefreshTokenError(string message) : Error(message)
{
}