using FluentResults;

namespace SocialNetwork.WebAPI.Errors;

public class InvalidCursorError(string message) : Error(message)
{}