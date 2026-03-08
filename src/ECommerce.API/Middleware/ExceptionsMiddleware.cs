using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace ECommerce.API.Middleware;

public class ExceptionsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _environment;
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _rateLimitWindow = TimeSpan.FromSeconds(30);
    private const int _maxRequests = 8;
    public ExceptionsMiddleware(
        RequestDelegate next, 
        IHostEnvironment environment, 
        IMemoryCache memoryCache)
    {
        _next = next;
        _environment = environment;
        _memoryCache = memoryCache;
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
        catch (Exception e)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            ApiExceptions response = _environment.IsDevelopment() ?
                new ApiExceptions((int)HttpStatusCode.InternalServerError, e.Message, e.StackTrace)
                : new ApiExceptions((int)HttpStatusCode.InternalServerError, e.Message);
            string json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }

    private bool IsRequestAllowed(HttpContext context)
    {
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