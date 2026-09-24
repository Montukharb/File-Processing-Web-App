using FileProcessing.Infrastructure.Background.EmailVerifyBackgroundTasks;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessing.Infrastructure.Background.Composition
{
    public static class DependencyInjection
    {
       public static IServiceCollection BackgroundServicesConfiguration(this IServiceCollection services)
        {
            services.AddHostedService<VerifyEmailBackground>();
            return services;
        }
    }
}
