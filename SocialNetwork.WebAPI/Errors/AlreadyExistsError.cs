using FluentResults;

namespace SocialNetwork.WebAPI.Errors;

public class AlreadyExistsError(string message): Error(message)
{
}