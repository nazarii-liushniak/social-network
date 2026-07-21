using FluentResults;

namespace SocialNetwork.WebAPI.Errors;

public class ValidationError(string message) : Error(message)
{
}