using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.RateLimiting;

namespace FileProcessing.Infrastructure.ApiRateLimiter.All_Limits.IP_Limit
{
    public static class Global_IP_Address_Rate_Limiter
    {
        public static RateLimiterOptions GlobalIpAddressRateLimiterOptions(this RateLimiterOptions options)
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(HttpContext =>
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknownIp";
                var endpoint = HttpContext.Request.Path.Value ?? "/";

                var globalBucketKey = $"ip{ip}_endpoint:{endpoint}";
                return RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey: globalBucketKey,
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
                    message = "Too many requests. g",
                    retryAfter = "2 Minutes"
                }, CancellationToken);
            };

            return options;
        }

    }
}







