using FileProcessing.Core.Email.Service;
using FileProcessing.Core.Email.Service.TemplateService;
using FileProcessing.Infrastructure.Email.Abstraction;
using FileProcessing.Infrastructure.Email.Abstraction.EmailTemplateAbstraction;
using FileProcessing.Infrastructure.Email.Service.TemplateService;
using Microsoft.Extensions.DependencyInjection;

namespace FileProcessing.Core.Email.Composition
{
    public static class DependencyInjection
    {
        public static IServiceCollection EmailConfiguration(this IServiceCollection services)
        {
            services.AddScoped<IAppLogoProvider, AppLogoProvider>();
            services.AddScoped<ISignUpTemplateService, SignupTemplateService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddRazorTemplating();
            return services;
        }
    }
}
