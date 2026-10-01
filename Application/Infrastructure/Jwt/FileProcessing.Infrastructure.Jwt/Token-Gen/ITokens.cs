using FileProcessing.Infrastructure.Jwt.Dto;
using FileProcessing.Infrastructure.Persistence.ApplicationUserManagement.Entitiy;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessing.Infrastructure.Jwt.Token_Gen
{
    public interface ITokens
    {
        public Task<string> AccessToken<TUser>(TUser user, string sessionId) where TUser : ApplicationUser;
        public Task<T> RefreshToken<T>() where T : IRefreshTokenDto, new();
    }
}
