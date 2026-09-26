using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Threading.RateLimiting;

namespace FileProcessing.Infrastructure.ApiRateLimiter.All_Limits.MultiLayer_Limit
{
    public static class MultilayerRateLimiter
    {
        public static RateLimiterOptions MultilayerRateLimiterOptions(this RateLimiterOptions options)
        {
            options.AddPolicy("Multilayer_Fixed", httpContext =>
            {
                // 1. IP aur User-Agent dono nikal lein
                var ipKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknownIp";
                var agentKey = httpContext.Request.Headers["User-Agent"].ToString();
                var userId = httpContext.User.Identity?.Name;
                // 2.  (Multi-layer track karne ke liye)
                var combinedKey = $"IP:{ipKey}_Agent:{agentKey}_User:{userId}";

                // 3. partition return
                return RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey: combinedKey,
                    factory: _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 20,
                        Window = TimeSpan.FromMinutes(2),
                        SegmentsPerWindow = 12,
                        QueueLimit = 0
                    });
            });
            options.OnRejected = async (context, CancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.Headers.RetryAfter = "2 Min";

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    statusCode = 429,
                    message = "Too many requests. m",
                    retryAfter = "2 Minutes"
                }, CancellationToken);
            };

            return options;
        }
    }
}