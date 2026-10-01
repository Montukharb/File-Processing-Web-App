using System.Net;
using System.Net.Http.Json;
using FileProcessing.Tests.Integration.Support;
using UserManagement.AM.RequestDTO;
using UserManagement.AM.ResponseDTO;

namespace FileProcessing.Tests.Integration.Api.UserManagement;

public sealed class ProfileEndpointTests
{
    [Fact]
    public async Task GetProfile_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var response = await client.GetAsync("/api/v1/auth/profile");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_WithAuthenticatedUser_ReturnsOnlyProfileFields()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            HandleCookies = false
        });
        await factory.InitializeDatabaseAsync();

        var signupResponse = await client.PostAsJsonAsync("/api/v1/auth/signup", ValidSignupRequest());
        signupResponse.EnsureSuccessStatusCode();

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequestDto
        {
            Email = "profile@example.com",
            Password = "StrongPass123!"
        });
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var accessCookie = Assert.Single(
            loginResponse.Headers.GetValues("Set-Cookie"),
            cookie => cookie.StartsWith("accessToken=", StringComparison.Ordinal));
        var accessCookiePair = accessCookie.Split(';', 2)[0];
        client.DefaultRequestHeaders.Add("Cookie", accessCookiePair);

        var response = await client.GetAsync("/api/v1/auth/profile");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var profile = await response.Content.ReadFromJsonAsync<ProfileResponseDto>();
        Assert.NotNull(profile);
        Assert.Equal("profile-user", profile.UserName);
        Assert.Equal("profile@example.com", profile.Email);
        Assert.False(profile.EmailConfirmed);

        var body = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("securityStamp", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("accessFailedCount", body, StringComparison.OrdinalIgnoreCase);
    }

    private static SignupRequestDto ValidSignupRequest() => new()
    {
        UserName = "profile-user",
        Email = "profile@example.com",
        Password = "StrongPass123!",
        ConfirmPassword = "StrongPass123!"
    };
}