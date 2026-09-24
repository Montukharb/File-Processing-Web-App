using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.RateLimiting;

namespace FileProcessing.Infrastructure.ApiRateLimiter.All_Limits.IP_Limit
{
    public static class IP_Address_Rate_Limiter
    {
        public static RateLimiterOptions IpAddressRateLimiterOptions(this RateLimiterOptions options)
        {
            options.AddPolicy("IP_Fixed", HttpContext =>
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknownIp";
                //var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                return RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey: ip,
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
                    message = "Too many requests.",
                    retryAfter = "2 Minutes",
                    ipaddress = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Not Found"
                }, CancellationToken);
            };

            return options;
        }

    }
}







