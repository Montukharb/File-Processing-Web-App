using FileProcessing.Infrastructure.Jwt.Authorization.Handler;
using FileProcessing.Infrastructure.Jwt.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessing.Infrastructure.Jwt.Authorization.Policies
{
    public static class Authorization_policy
    {
        public static void AuthoriationPolicy(this AuthorizationOptions options)
        {
            options.AddPolicy("User", policy => policy.RequireAuthenticatedUser());

            //General Policy
            options.AddPolicy("policyName", policy =>
            {
                policy.RequireClaim("Department", "IT");
                policy.RequireRole("Admin", "Emp");
            });

            //Custom Policy Requirements
            options.AddPolicy("Custom", policy =>
            {
                policy.Requirements.Add(new CustomRequirementsSample("India"));
            });
        }
    }
}
