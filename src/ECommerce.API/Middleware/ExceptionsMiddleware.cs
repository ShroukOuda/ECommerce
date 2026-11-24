using System.Net;
using System.Text.Json;
using ECommerce.Domain.Exceptions;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;

namespace ECommerce.API.Middleware;

public class ExceptionsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _environment;
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _rateLimitWindow = TimeSpan.FromSeconds(30);
    private readonly int _maxRequests;
    public ExceptionsMiddleware(
        RequestDelegate next, 
        IHostEnvironment environment, 
        IMemoryCache memoryCache, IConfiguration configuration)
    {
        _next = next;
        _environment = environment;
        _memoryCache = memoryCache;
        _maxRequests = Math.Max(1, configuration.GetValue<int?>("RateLimiting:MaxRequests") ?? 8);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            ApplySecurity(context);
            
            if (IsRequestAllowed(context) == false)
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.ContentType = "application/json";
                
                ApiExceptions response = new ApiExceptions((int)HttpStatusCode.TooManyRequests, "Too many request, please try again later");
                string json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);

                return;
            }
            await _next(context);
        }
        catch (KeyNotFoundException e)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.ContentType = "application/json";
            ApiExceptions notFoundResponse = new ApiExceptions((int)HttpStatusCode.NotFound, e.Message);
            string notFoundJson = JsonSerializer.Serialize(notFoundResponse);
            await context.Response.WriteAsync(notFoundJson);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            context.Response.StatusCode = 499;
        }
        catch (Exception e)
        {
            var statusCode = GetStatusCode(e);
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            ApiExceptions response = _environment.IsDevelopment() ?
                new ApiExceptions(statusCode, e.Message, e.StackTrace)
                : new ApiExceptions(statusCode, statusCode >= 500 ? "An unexpected error occurred." : e.Message);
            string json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }

    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            BusinessRuleException => (int)HttpStatusCode.BadRequest,
            ConflictException => (int)HttpStatusCode.Conflict,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            ValidationException => (int)HttpStatusCode.BadRequest,
            BadRequestException => (int)HttpStatusCode.BadRequest,
            NotFoundException => (int)HttpStatusCode.NotFound,
            UnauthorizedAccessException => (int)HttpStatusCode.Forbidden,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };
    }

    private bool IsRequestAllowed(HttpContext context)
    {
        // Stripe authenticates its own delivery with a signed payload. Shared-IP UI quotas must not drop webhooks.
        if (context.Request.Path.Equals("/api/v1/payments/webhook", StringComparison.OrdinalIgnoreCase)
            || context.Request.Path.Equals("/api/v1/health", StringComparison.OrdinalIgnoreCase)) return true;
        string ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        string cacheKey = $"RateLimit: {ip}";
        DateTime dateNow = DateTime.Now;

        var (timesTamp, count) = _memoryCache.GetOrCreate(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _rateLimitWindow;
            return (timesTamp: dateNow, count: 0);
        });

        if (dateNow - timesTamp < _rateLimitWindow)
        {
            if (count >= _maxRequests)
                return false;
            _memoryCache.Set(cacheKey, (timesTamp, count + 1), _rateLimitWindow);
        }
        else
        {
            _memoryCache.Set(cacheKey, (dateNow, 1), _rateLimitWindow);
        }

        return true;
    }

    private void ApplySecurity(HttpContext context)
    {
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
        context.Response.Headers["X-Frame-Options"] = "DENY";
    }
    
    
}
