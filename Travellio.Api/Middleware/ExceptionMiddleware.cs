using System.Net;
using System.Text.Json;
using Serilog.Context;

namespace Travellio.Api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var endpoint = context.GetEndpoint() as RouteEndpoint;
            var routePattern = endpoint?.RoutePattern.RawText ?? context.Request.Path;


            using (LogContext.PushProperty("RouteTemplate", routePattern))
            {
                logger.LogError(ex, "Unhandled exception for {Method} {RequestPath}", context.Request.Method,
                    context.Request.Path);
            }

            await WriteErrorResponse(context);
        }
    }

    private static Task WriteErrorResponse(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var body = JsonSerializer.Serialize(new { error = "An unexpected error occurred." });
        return context.Response.WriteAsync(body);
    }
}