using FluentResults;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.WebAPI.Errors;

namespace SocialNetwork.WebAPI.Extensions;

public static class ResultExtensions
{
    public static ActionResult ToActionResult(this Result result)
    {
        // result.HasError<AlreadyExistsError>() ensures idempotency for follow and like creation
        if (result.IsSuccess || result.HasError<AlreadyExistsError>())
            return new NoContentResult();
        
        return HandleError(result.Errors[0]);
    }

    public static ActionResult<T> ToActionResult<T>(this Result<T> result, Func<T, ActionResult<T>>? onSuccess = null)
    {
        if (result.IsSuccess)
            return onSuccess == null
                ? new OkObjectResult(result.Value)
                : onSuccess.Invoke(result.Value);

        return HandleError(result.Errors[0]);
    }

    private static ActionResult HandleError(IError error)
    {
        var statusCode = error switch
        {
            ExpiredRefreshTokenError or InvalidCursorError
                or InvalidRefreshTokenError or ValidationError => StatusCodes.Status400BadRequest,
            AuthenticationError => StatusCodes.Status401Unauthorized,
            ForbiddenError => StatusCodes.Status403Forbidden,
            NotFoundError => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError,
        };

        var problemDetails = new ProblemDetails()
        {
            Status = statusCode,
            Title = error.GetType().Name,
            Detail = error.Message,
        };

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode,
        };
    }
}