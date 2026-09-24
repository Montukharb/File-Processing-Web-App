using FileProcessing.Infrastructure.Cache.Abstraction;
using FileProcessing.Infrastructure.Cache.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessing.Infrastructure.Cache.Composition
{
    public static class DepencyInjection
    {
        public static IServiceCollection CacheConfiguration(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["Cache:Redis"];
                options.InstanceName = "FileProcessing"; //get and set key look like this
                //"FileProcessing:User:123" FileProcessing auto add every request get or set

            });
            service.AddScoped<ICacheProcessKEngine, ManagedCache>();
            return service;
        }
    }
}
