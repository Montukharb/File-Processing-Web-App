using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessing.Infrastructure.Jwt.CookieSetting
{
    public interface ICookieConfiguration
    {

        public CookieOptions RefreshTokenCookieOptions();
        public CookieOptions AccesTokenCookieOptions();
    }
}
