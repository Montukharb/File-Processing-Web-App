using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.RateLimiting;

namespace FileProcessing.Infrastructure.ApiRateLimiter.All_Limits.IP_And_User_Agent_Limit
{
    public static class Ip_UserAgent_Rate_LImiter
    {
        public static RateLimiterOptions IpAddressAndUserAgentRateLimiterOptions(this RateLimiterOptions options)
        {
            options.AddPolicy("IP_AND_USERAGENT_FIXED", HttpContext =>
            {
                string IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknownIp";
                var UserAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                var key = IpAddress + ":" + UserAgent;
                return RateLimitPartition.GetSlidingWindowLimiter(

                    partitionKey: key,
                    factory: _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 20,
                        Window = TimeSpan.FromMinutes(2),
                        SegmentsPerWindow = 12,
                        QueueLimit = 0,

                    });
            });
            options.OnRejected = async (context, CancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.Headers.RetryAfter = "2 Min";

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    statusCode = 429,
                    message = "Too many requests. i-u-a",
                    retryAfter = "2 Minutes"
                }, CancellationToken);
            };

            return options;
        }
    }
}
