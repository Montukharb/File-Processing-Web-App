using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using FileProcessing.Core.CommonLibrary.Exceptions;
using FileProcessing.Infrastructure.Email.Settings;
using FileProcessing.Core.Email.Composition;
using FileProcessing.Infrastructure.TaskQueue.Composition;
using FileProcessing.Infrastructure.Background.Composition;
using FileProcessing.Infrastructure.Cache.Composition;
using FileProcessing.Infrastructure.ApiRateLimiter.Composition;
using UserManagement.PL.Composition;
namespace FileProcessing.WebSolution.ModuleComposition
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddModuleComposition(this IServiceCollection services, IConfiguration configuration)
        {
            /*     
               OptionsConfigurationServiceCollectionExtensions
                     .Configure<EmailSetting>(
                      services,
                     configuration.GetSection("EmailSettings"));*/
            //builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("EmailSettings"));
            services.UserManagementDependencyInjection();
            services.AddTransient<GlobalExceptionHandler>();
            services.Configure<EmailSetting>(configuration.GetSection("EmailSettings"));
            services.EmailVerifyConfiguration();
            services.BackgroundServicesConfiguration();
            services.RateLimitingConfigure();
            services.CacheConfiguration(configuration);
            services.EmailConfiguration(); //Email Di Register
            return services;
        }
    }
}
