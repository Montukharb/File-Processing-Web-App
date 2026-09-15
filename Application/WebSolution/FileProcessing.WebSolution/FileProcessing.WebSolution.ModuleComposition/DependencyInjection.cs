using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using FileProcessing.Core.CommonLibrary.Exceptions;
using FileProcessing.Infrastructure.Email.Settings;
using FileProcessing.Core.Email.Composition;

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
            services.AddTransient<GlobalExceptionHandler>();
            services.Configure<EmailSetting>(configuration.GetSection("EmailSettings"));
            services.EmailConfiguration(); //Email Di Register
            return services;
        }
    }
}
