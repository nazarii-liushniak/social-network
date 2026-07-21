using FluentResults;

namespace SocialNetwork.WebAPI.Errors;

public class ExpiredRefreshTokenError(string message) : Error(message)
{}