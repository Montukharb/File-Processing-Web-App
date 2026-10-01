using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FileProcessing.Infrastructure.Persistence;
using FileProcessing.Infrastructure.Persistence.ApplicationUserManagement.Entitiy;
using FileProcessing.Tests.Integration.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.AM.RequestDTO;

namespace FileProcessing.Tests.Integration.Api.UserManagement;

public sealed class UserSessionEndpointTests
{
    [Fact]
    public async Task SessionEndpoints_PersistRotateAndRevokeSessions()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            HandleCookies = false
        });
        await factory.InitializeDatabaseAsync();

        var signupResponse = await client.PostAsJsonAsync("/api/v1/auth/signup", ValidSignupRequest());
        signupResponse.EnsureSuccessStatusCode();

        var firstLogin = await LoginAsync(client);
        var secondLogin = await LoginAsync(client);
        var firstRefreshToken = firstLogin.RefreshToken;
        var secondRefreshToken = secondLogin.RefreshToken;

        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var sessions = await dbContext.Set<UserSession>().OrderBy(session => session.CreatedAt).ToListAsync();
            Assert.Equal(2, sessions.Count);
            Assert.All(sessions, session =>
            {
                Assert.NotEqual(firstRefreshToken, session.RefreshTokenHash);
                Assert.NotEqual(secondRefreshToken, session.RefreshTokenHash);
                Assert.Equal(64, session.RefreshTokenHash.Length);
            });
        }

        var refreshResponse = await SendWithCookieAsync(
            client,
            HttpMethod.Post,
            "/api/v1/auth/refresh-token",
            $"refreshToken={secondRefreshToken}");
        Assert.Equal(HttpStatusCode.NoContent, refreshResponse.StatusCode);
        var rotatedRefreshToken = GetCookieValue(refreshResponse, "refreshToken");
        Assert.NotEqual(secondRefreshToken, rotatedRefreshToken);

        var replayResponse = await SendWithCookieAsync(
            client,
            HttpMethod.Post,
            "/api/v1/auth/refresh-token",
            $"refreshToken={secondRefreshToken}");
        Assert.Equal(HttpStatusCode.Unauthorized, replayResponse.StatusCode);

        var terminateResponse = await SendWithCookieAsync(
            client,
            HttpMethod.Post,
            "/api/v1/auth/sessions/terminate-others",
            firstLogin.AccessCookie);
        Assert.Equal(HttpStatusCode.OK, terminateResponse.StatusCode);
        Assert.Contains("\"terminatedSessions\":1", await terminateResponse.Content.ReadAsStringAsync());

        var terminatedProfileResponse = await SendWithCookieAsync(
            client,
            HttpMethod.Get,
            "/api/v1/auth/profile",
            secondLogin.AccessCookie);
        Assert.Equal(HttpStatusCode.Unauthorized, terminatedProfileResponse.StatusCode);

        var logoutResponse = await SendWithCookieAsync(
            client,
            HttpMethod.Post,
            "/api/v1/auth/logout",
            firstLogin.AccessCookie);
        Assert.Equal(HttpStatusCode.NoContent, logoutResponse.StatusCode);

        var loggedOutProfileResponse = await SendWithCookieAsync(
            client,
            HttpMethod.Get,
            "/api/v1/auth/profile",
            firstLogin.AccessCookie);
        Assert.Equal(HttpStatusCode.Unauthorized, loggedOutProfileResponse.StatusCode);
    }

    private static async Task<(string AccessCookie, string RefreshToken)> LoginAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequestDto
        {
            Email = "sessions@example.com",
            Password = "StrongPass123!"
        });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (GetCookieValue(response, "accessToken"), GetCookieValue(response, "refreshToken"));
    }

    private static async Task<HttpResponseMessage> SendWithCookieAsync(
        HttpClient client,
        HttpMethod method,
        string path,
        string cookie)
    {
        using var request = new HttpRequestMessage(method, path);
        request.Headers.Add("Cookie", cookie.Contains('=') ? cookie : $"accessToken={cookie}");
        return await client.SendAsync(request);
    }

    private static string GetCookieValue(HttpResponseMessage response, string name)
    {
        var cookie = Assert.Single(
            response.Headers.GetValues("Set-Cookie"),
            value => value.StartsWith($"{name}=", StringComparison.Ordinal));
        return cookie.Split(';', 2)[0][(name.Length + 1)..];
    }

    private static SignupRequestDto ValidSignupRequest() => new()
    {
        UserName = "sessions-user",
        Email = "sessions@example.com",
        Password = "StrongPass123!",
        ConfirmPassword = "StrongPass123!"
    };
}