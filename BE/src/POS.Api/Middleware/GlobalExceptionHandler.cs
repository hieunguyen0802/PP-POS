using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using POS.Domain.Exceptions;
using Grpc.Core;

namespace POS.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = MapException(exception);

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            // Log the full exception for unexpected errors; the client only ever sees a generic message.
            _logger.LogError(exception, "Unhandled exception occurred.");
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };



        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions["errors"] = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static (int StatusCode, string Title, string Detail) MapException(Exception exception)
    {
        return exception switch
        {
            DomainExceptions.EntityNotFound => (StatusCodes.Status404NotFound, "Not Found", exception.Message),
            DomainExceptions.EntityAlreadyExists => (StatusCodes.Status409Conflict, "Conflict", exception.Message),
            DomainExceptions.InvalidOrderStatus => (StatusCodes.Status400BadRequest, "Invalid Operation", exception.Message),
            DomainExceptions.InsufficientStock => (StatusCodes.Status400BadRequest, "Invalid Operation", exception.Message),
            RpcException { StatusCode: StatusCode.Unavailable } => (StatusCodes.Status503ServiceUnavailable, "Service Unavailable", "The inventory service is currently unavailable. Please try again shortly."),
            RpcException { StatusCode: StatusCode.Aborted } => (StatusCodes.Status409Conflict, "Conflict", "Stock was being updated by another request. Please try again."),
            RpcException => (StatusCodes.Status502BadGateway, "Bad Gateway", "The inventory service returned an unexpected error."),
            ValidationException => (StatusCodes.Status400BadRequest, "Validation Failed", "One or more validation errors occurred."),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred. Please try again later.")
        };
    }
}
