using Aruba.Document.Application.Exceptions;
using Aruba.Document.Domain.Exceptions;
using FluentValidation;

namespace Aruba.Document.Api.Common.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException exception)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var errors = exception.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });

            await context.Response.WriteAsJsonAsync(new { Errors = errors });
        }
        catch (DomainException exception)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new { Error = exception.Message });
        }
        catch (NotFoundException exception)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new { Error = exception.Message });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new { Error = "An unexpected error occurred." });
        }
    }
}