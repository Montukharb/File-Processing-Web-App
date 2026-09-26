using FileProcessing.Infrastructure.Jwt.Dto;
using FileProcessing.Infrastructure.Persistence.ApplicationUserManagement.Entitiy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FileProcessing.Infrastructure.Jwt.Token_Gen
{
    public class Tokens : ITokens
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IConfiguration _configuration;

        public Tokens(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [NonAction]
        public async Task<string> AccessToken<TUser>(TUser user) where TUser : ApplicationUser
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role,"User")
            };

            //Add All Roles
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                new Claim(ClaimTypes.Role, role);
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            //role claims
            foreach (var roleName in roles)
            {
                var Roleclaims = await _roleManager.FindByNameAsync(roleName);
                if (Roleclaims != null)
                {
                    var allClaims = await _roleManager.GetClaimsAsync(Roleclaims); //array
                    claims.AddRange(allClaims);
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));

            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                  issuer: _configuration["jwt:issuer"],
                  audience: _configuration["Jwt:audience"],
                  claims: claims,
                  expires: DateTime.UtcNow.AddMinutes(20),
                  signingCredentials: credential
                 );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [NonAction]
        public Task<T> RefreshToken<T>() where T : IRefreshTokenDto, new()
        {
            byte[] bytes = RandomNumberGenerator.GetBytes(32);
            var token = Convert.ToBase64String(bytes);
            var created = DateTime.UtcNow;
            var Expires = DateTime.UtcNow.AddDays(7);
            var refreshToken = new T
            {
                RFToken = token,
                RFCreatedToken = created,
                RFExpireToken = Expires
            };
            return Task.FromResult(refreshToken);
        }
    }
}
