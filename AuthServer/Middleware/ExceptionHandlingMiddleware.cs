using System.Net;
using System.Text.Json;
using AuthServer.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Middleware;

/// <summary>
/// Middleware that handles exceptions and converts them to ProblemDetails responses
/// </summary>
public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger
)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OAuthException ex)
        {
            logger.LogWarning(ex, "OAuth error occurred: {ErrorCode}", ex.ErrorCode);
            await HandleOAuthExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context);
        }
    }

    private static async Task HandleOAuthExceptionAsync(
        HttpContext context,
        OAuthException exception
    )
    {
        context.Response.StatusCode = (int)exception.StatusCode;
        context.Response.ContentType = "application/json";

        var problemDetails = new ProblemDetails
        {
            Status = (int)exception.StatusCode,
            Title = exception.Message,
            Type = $"https://tools.ietf.org/html/rfc6749#section-5.2",
            Extensions = new Dictionary<string, object?>
            {
                { "error", exception.ErrorCode },
                { "error_description", exception.ErrorDescription },
                { "error_uri", exception.ErrorUri },
            },
        };

        var json = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(json);
    }

    private static async Task HandleExceptionAsync(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "An error occurred while processing your request.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
        };

        var json = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(json);
    }
}
