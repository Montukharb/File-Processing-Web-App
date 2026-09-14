using FileProcessing.Core.Email.Abstraction;
using FileProcessing.Core.Email.Abstraction.EmailTemplateAbstraction;
using FileProcessing.Core.Email.Service;
using FileProcessing.Core.Email.Service.TemplateService;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

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
