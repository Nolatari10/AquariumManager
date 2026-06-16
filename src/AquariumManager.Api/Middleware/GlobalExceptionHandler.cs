using System.Net;
using System.Text.Json;

namespace AquariumManager.Api.Middleware;

public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (IsDatabaseUnavailable(ex))
            {
                _logger.LogError(ex, "Database is unavailable or connection was refused");
            }
            else
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            }
            await HandleExceptionAsync(context, ex);
        }
    }

    private static bool IsDatabaseUnavailable(Exception ex)
    {
        return WalkExceptions(ex);

        static bool WalkExceptions(Exception? e)
        {
            while (e is not null)
            {
                var typeName = e.GetType().FullName ?? "";
                if (typeName.StartsWith("Npgsql.", StringComparison.Ordinal))
                    return true;
                if (e is System.Net.Sockets.SocketException)
                    return true;
                if (e is InvalidOperationException && e.Message.Contains("transient failure", StringComparison.OrdinalIgnoreCase))
                    return true;
                e = e.InnerException;
            }
            return false;
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            StatusCode = (int)HttpStatusCode.InternalServerError,
            Message = "An unexpected error occurred. Please try again later.",
            Detail = exception.Message
        };

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);

        return context.Response.WriteAsync(json);
    }
}
