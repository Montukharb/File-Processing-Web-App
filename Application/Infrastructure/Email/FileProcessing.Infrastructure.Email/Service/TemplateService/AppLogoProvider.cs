using FileProcessing.Infrastructure.Email.Abstraction.EmailTemplateAbstraction;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessing.Infrastructure.Email.Service.TemplateService
{
    public class AppLogoProvider(IWebHostEnvironment _Environment) : IAppLogoProvider
    {

        public Task<byte[]> GetAppLightLogoAsync()
        {
            string path = Path.Combine(_Environment.WebRootPath, "Logo", "App_Light_Logo.png");

            return File.ReadAllBytesAsync(path);
        }
        public Task<byte[]> GetAppDarkLogoAsync()
        {
            string path = Path.Combine(_Environment.WebRootPath, "Logo", "App_Dark_Logo.png");

            return File.ReadAllBytesAsync(path);
        }
    }
}
