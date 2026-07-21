using FluentResults;

namespace SocialNetwork.WebAPI.Errors;

public class AuthenticationError(string message) : Error(message)
{}