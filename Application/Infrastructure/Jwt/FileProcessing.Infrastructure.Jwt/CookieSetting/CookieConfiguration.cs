using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessing.Infrastructure.Jwt.CookieSetting
{
    public class CookieConfiguration(IWebHostEnvironment Env) : ICookieConfiguration
    {
        public CookieOptions RefreshTokenCookieOptions()
        {
            return new CookieOptions
            {
                HttpOnly = true, //java script protection
                Secure = Env.IsDevelopment() ? false : true, // https required
                SameSite = Env.IsDevelopment() ? SameSiteMode.None : SameSiteMode.Lax, //csrf attack protection 
                Expires = DateTime.UtcNow.AddDays(7),
                Path = "/api/v1/auth/refresh-token"
            };

            //Response.Cookies.Append("refreshToken", refreshTokenValue, cookieOptions);
        }

        public CookieOptions AccesTokenCookieOptions()
        {
            return new CookieOptions
            {
                HttpOnly = true, //java script protection
                Secure = Env.IsDevelopment() ? false : true, // https required
                SameSite = Env.IsDevelopment() ? SameSiteMode.None : SameSiteMode.Lax, //csrf attack protection 
                Expires = DateTime.UtcNow.AddDays(7),
                Domain = Env.IsDevelopment() ? string.Empty : ".yourcompany.com"
            };

            //Response.Cookies.Append("accessToken", refreshTokenValue, cookieOptions);
        }
    }
}
