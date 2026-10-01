using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using UserManagement.Web;

namespace FileProcessing.WebSolution.ModuleComposition
{
    public static class DependencyInjectionWeb
    {
       public static WebApplication ModulesWebDI(this WebApplication app)
        {

            app.UseHttpsRedirection(); //http request convert into https request
            app.UseRateLimiter(); // Rate Limiter Middleware
            app.MapControllers(); // Map the controllers to the request pipeline
            app.UseDefaultFiles(); //Find and match the default file like index.html etc.
            app.MapStaticAssets(); // Map the static assets folder to the request path "/assets"
            app.UseStatusCodePages(); // Handle status code pages for 404, 500, etc.

            //All Modules Web library Registrations
            app.MapUserAuthenticationEndpoints();
            return app;
        }
    }
}
