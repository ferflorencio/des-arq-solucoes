using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SolutionArchitect.CashFlow.Consolidate.Api.Shareable.Exceptions;
using System.Data;

namespace SolutionArchitect.CashFlow.Consolidate.ApiService.Extensions;


public static class ExceptionHandlingExtensions
{
    public static void UseApiExceptionHandling(this WebApplication app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var feature =
                    context.Features.Get<IExceptionHandlerFeature>();

                var exception = feature?.Error;

                if (exception is null)
                    return;

                var problem = exception switch
                {
                    InvalidOperationTypeException e => CreateProblem(
                        StatusCodes.Status400BadRequest,
                        "Invalid request",
                        e.Message),

                    RequestDataInvalidException e => CreateProblem(
                        StatusCodes.Status400BadRequest,
                        "Invalid request data",
                        "One or more validation errors occurred.",
                        new Dictionary<string, object?>
                        {
                            ["errors"] = e.Errors
                        }),

                    NotFoundException e => new ProblemDetails
                    {
                        Title = "Resource not found",
                        Detail = e.Message,
                        Status = StatusCodes.Status404NotFound
                    },

                    ConcurrencyException e => CreateProblem(
                        StatusCodes.Status409Conflict,
                        "Concurrency conflict",
                        e.Message),

                    AppException e => CreateProblem(
                        StatusCodes.Status422UnprocessableEntity,
                        "Business rule violation",
                        e.Message),

                    _ => CreateProblem(
                        StatusCodes.Status500InternalServerError,
                        "Internal server error",
                        "An unexpected error occurred.")
                };

                context.Response.StatusCode = problem.Status!.Value;
                await context.Response.WriteAsJsonAsync(problem);
            });
        });
    }

    private static ProblemDetails CreateProblem(
        int status,
        string title,
        string detail,
        IDictionary<string, object?>? extensions = null)
    {
        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail
        };

        if (extensions is not null)
        {
            foreach (var kv in extensions)
                problem.Extensions[kv.Key] = kv.Value;
        }

        return problem;
    }
}
