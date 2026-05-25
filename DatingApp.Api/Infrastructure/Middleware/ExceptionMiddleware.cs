using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace DatingApp.Api.Infrastructure.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            ApiExceptionModel response = _env.IsDevelopment()
                ? new ApiExceptionModel(context.Response.StatusCode, e.Message, e.StackTrace)
                : new ApiExceptionModel(context.Response.StatusCode, "Internal Server Error");

            JsonSerializerOptions options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
            string json = JsonSerializer.Serialize(response, options);

            _logger.LogError(e, "Request pipeline error\n{Json}", json);

            await context.Response.WriteAsync(json);
        }
    }
}
