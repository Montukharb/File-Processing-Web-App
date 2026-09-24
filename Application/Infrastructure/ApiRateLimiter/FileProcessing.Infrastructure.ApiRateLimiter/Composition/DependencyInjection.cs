using FileProcessing.Infrastructure.ApiRateLimiter.All_Limits.IP_Limit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessing.Infrastructure.ApiRateLimiter.Composition
{
    public static class DependencyInjection
    {
        public static IServiceCollection RateLimitingConfigure(this IServiceCollection services)
        {

            services.AddRateLimiter(options =>
            {
                options.IpAddressRateLimiterOptions();
                
            });
            return services;
        }
    }
}
