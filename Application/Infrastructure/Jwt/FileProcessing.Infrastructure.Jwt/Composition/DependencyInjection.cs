using FileProcessing.Infrastructure.Jwt.Auth;
using FileProcessing.Infrastructure.Jwt.Authorization.Policies;
using FileProcessing.Infrastructure.Jwt.CookieSetting;
using FileProcessing.Infrastructure.Jwt.Token_Gen;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessing.Infrastructure.Jwt.Composition
{
    public static class DependencyInjection
    {
        public static IServiceCollection AuthConfiguration(this IServiceCollection services, IConfiguration builder)
        {
            services.AddScoped<ITokens, Tokens>();
            services.AddScoped<ICookieConfiguration, CookieConfiguration>();
            services.AuthenticationBuilderConfigure(builder); //authentication
            services.AddAuthorization(options => options.AuthoriationPolicy()); //authorization policies
            services.AddSingleton<ICookieConfiguration, CookieConfiguration>();
            return services;
        }
    }
}
