using System.Security.Cryptography;
using System.Text;

namespace FileProcessing.Infrastructure.Jwt.Token_Gen;

public static class RefreshTokenHash
{
    public static string Compute(string refreshToken)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
    }
}