using FileProcessing.Infrastructure.Jwt.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using System.Text;

namespace FileProcessing.Infrastructure.Jwt.Authorization.Handler
{
    public sealed class CustomRequirementsHandler(ILogger<CustomRequirementsHandler> logger) : AuthorizationHandler<CustomRequirementsSample>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRequirementsSample requirement)
        {
            try
            {

                var targetClaim = context.User.HasClaim("Country", requirement.Country.ToUpper());
                if (targetClaim)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    logger.LogInformation("Can't find user Requirements");
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Exception occur during custom authorization hander {Error}:", ex);
            }
            return Task.CompletedTask;
        }
    }
}

/*
| Method      | Work                      |
| ----------- | ------------------------- |
| IsInRole()  | Role check                |
| FindFirst() | First claim               |
| FindAll()   | Same type ke saare claims |
| HasClaim()  | Claim exist karta hai?    |
| Claims      | All claims                |
| Identity    | Login info                |
*/
/*var role = context.User.IsInRole("Admin,Manager");
var claim = context.User.HasClaim("Country", requirement.Country);
bool isAdmin = context.User.IsInRole("Admin");
var claims = context.User.Claims;
foreach (var claim in claims)
{
    Console.WriteLine($"Claim Type = " + claim.Type + " ,Claim Value = " + claim.Value);
}*/