using System.Net;
using System.Net.Http.Json;
using FileProcessing.Infrastructure.Email.Models;
using FileProcessing.Infrastructure.Persistence.ApplicationUserManagement.Entitiy;
using FileProcessing.Infrastructure.TaskQueue.SignupEmailVerifyTask;
using FileProcessing.Tests.Integration.Support;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.AM.RequestDTO;
using UserManagement.AM.ResponseDTO;

namespace FileProcessing.Tests.Integration.Api.UserManagement;

public sealed class SignupAndEmailConfirmationEndpointTests
{
    [Fact]
    public async Task Signup_CreatesAccountAndQueuesVerificationEmail()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var response = await client.PostAsJsonAsync("/api/v1/auth/signup", ValidSignupRequest());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var signup = await response.Content.ReadFromJsonAsync<SignupResponseDto>();
        Assert.NotNull(signup);
        Assert.True(signup.Success);
        Assert.Equal("signup@example.com", signup.Email);

        var email = Assert.IsType<UserSignUpModel>(await factory.EmailVerifyQueue.ConsumeJob());
        Assert.Equal("signup@example.com", email.To);
        Assert.StartsWith("http://localhost/api/v1/auth/verify-email?", email.ActionUrl);
    }

    [Fact]
    public async Task VerifyEmail_WithQueuedToken_ConfirmsUserEmail()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var signupResponse = await client.PostAsJsonAsync("/api/v1/auth/signup", ValidSignupRequest());
        signupResponse.EnsureSuccessStatusCode();

        var email = Assert.IsType<UserSignUpModel>(await factory.EmailVerifyQueue.ConsumeJob());
        var emailUrl = new Uri(email.ActionUrl);
        var query = QueryHelpers.ParseQuery(emailUrl.Query);
        var confirmationPath = QueryHelpers.AddQueryString(
            "/api/v1/auth/verify-email",
            new Dictionary<string, string?>
            {
                ["userId"] = query["userId"].ToString(),
                ["token"] = query["token"].ToString()
            });

        var confirmationResponse = await client.GetAsync(confirmationPath);

        Assert.Equal(HttpStatusCode.OK, confirmationResponse.StatusCode);
        var confirmation = await confirmationResponse.Content.ReadFromJsonAsync<EmailConfirmationResponseDto>();
        Assert.NotNull(confirmation);
        Assert.True(confirmation.Success);
        Assert.Equal("signup@example.com", confirmation.Email);

        await using var scope = factory.Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.FindByEmailAsync("signup@example.com");
        Assert.NotNull(user);
        Assert.True(user.EmailConfirmed);
    }

    [Fact]
    public async Task Signup_WithDuplicateEmail_ReturnsConflict()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var firstResponse = await client.PostAsJsonAsync("/api/v1/auth/signup", ValidSignupRequest());
        firstResponse.EnsureSuccessStatusCode();

        var duplicateResponse = await client.PostAsJsonAsync("/api/v1/auth/signup", ValidSignupRequest());

        Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
    }

    private static SignupRequestDto ValidSignupRequest() => new()
    {
        UserName = "signup-user",
        Email = "signup@example.com",
        Password = "StrongPass123!",
        ConfirmPassword = "StrongPass123!"
    };
}