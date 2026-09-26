using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessing.Infrastructure.Jwt.Authorization.Requirements
{
    public sealed class CustomRequirementsSample : IAuthorizationRequirement
    {
        public string Country { get; init; }
        public CustomRequirementsSample(string Country)
        {
            this.Country = Country;

        }
    }
}
