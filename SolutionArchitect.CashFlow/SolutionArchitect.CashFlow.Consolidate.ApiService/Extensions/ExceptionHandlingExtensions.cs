using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SolutionArchitect.CashFlow.Consolidate.Api.Shareable.Exceptions;

namespace SolutionArchitect.CashFlow.Consolidate.ApiService.Extensions;

public static class ExceptionHandlingExtensions
{
    public static void UseDomainExceptionHandling(this WebApplication app)
    {
        app.UseExceptionHandler(handler =>
        {
            handler.Run(async context =>
            {
                var exception =
                    context.Features.Get<IExceptionHandlerFeature>()?.Error;

                if (exception is null)
                    return;

                var problemDetails = exception switch
                {
                    RequestDataInvalidException e => new ProblemDetails
                    {
                        Title = "Invalid request",
                        Detail = "One or more validation errors occurred.",
                        Status = StatusCodes.Status400BadRequest,
                        Extensions =
                        {
                            ["errors"] = e.Errors
                        }
                    },

                    NotFoundException e => new ProblemDetails
                    {
                        Title = "Resource not found",
                        Detail = e.Message,
                        Status = StatusCodes.Status404NotFound
                    },

                    AppException e => new ProblemDetails
                    {
                        Title = "Business rule violation",
                        Detail = e.Message,
                        Status = StatusCodes.Status422UnprocessableEntity
                    },

                    LockException e => new ProblemDetails
                    {
                        Title = "Concurrent update detected.",
                        Detail = e.Message,
                        Status = StatusCodes.Status422UnprocessableEntity
                    },

                    InvalidOperationTypeException e => new ProblemDetails
                    {
                        Title = "Invalid operation type",
                        Detail = e.Message,
                        Status = StatusCodes.Status400BadRequest
                    },

                    _ => new ProblemDetails
                    {
                        Title = "Internal server error",
                        Detail = "An unexpected error occurred.",
                        Status = StatusCodes.Status409Conflict
                    }
                };

                context.Response.StatusCode = problemDetails.Status!.Value;
                await context.Response.WriteAsJsonAsync(problemDetails);
            });
        });
    }
}
