using Microsoft.AspNetCore.Authentication.JwtBearer;
using FileProcessing.Infrastructure.Persistence;
using FileProcessing.Infrastructure.Persistence.ApplicationUserManagement.Entitiy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace FileProcessing.Infrastructure.Jwt.Auth
{
    public static class Authentication_Builder
    {
        public static IServiceCollection AuthenticationBuilderConfigure(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = config["Jwt:issuer"],
                    ValidAudience = config["Jwt:audience"],

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:key"]!))

                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (!context.Request.Headers.ContainsKey("Authorization") &&
                            context.Request.Cookies.TryGetValue("accessToken", out var accessToken))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },

                    OnTokenValidated = async context =>
                    {
                        var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        var sessionId = context.Principal?.FindFirst(ClaimTypes.Sid)?.Value;
                        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(sessionId))
                        {
                            context.Fail("The token is not associated with a valid session.");
                            return;
                        }

                        var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                        var session = await dbContext.Set<UserSession>()
                            .AsNoTracking()
                            .SingleOrDefaultAsync(
                                item => item.UserId == userId && item.SessionId == sessionId,
                                context.HttpContext.RequestAborted);

                        if (session is null || session.IsRevoked || session.RevokedAt.HasValue || session.ExpiresAt <= DateTime.UtcNow)
                        {
                            context.Fail("The session is no longer active.");
                        }
                    }
                };
            });
            return services;
        }
    }
}
