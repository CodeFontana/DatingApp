using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next,
                               ILogger<ExceptionMiddleware> logger,
                               IHostEnvironment env)
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
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            ApiExceptionModel response = _env.IsDevelopment()
                ? new ApiExceptionModel(context.Response.StatusCode, e.Message, e.StackTrace?.ToString())
                : new ApiExceptionModel(context.Response.StatusCode, "Internal Server Error");

            JsonSerializerOptions options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
            string json = JsonSerializer.Serialize(response, options);

            _logger.LogError(e, "Request pipeline error\n{json}", FormatJson(json));

            await context.Response.WriteAsync(json);
        }
    }

    private static string FormatJson(string message)
    {
        string result;

        if (message.Contains("\r\n") || message.Contains('\n'))
        {
            string[] splitMsg = message
                .Replace("\\r\\n", "\n")
                .Replace("\r\n", "\n")
                .Split(new char[] { '\n' });

            result = string.Join(Environment.NewLine, splitMsg);
        }
        else
        {
            result = message;
        }

        return result;
    }
}
