using FileProcessing.Infrastructure.Email.Abstraction.EmailTemplateAbstraction;
using FileProcessing.Infrastructure.Persistence;
using FileProcessing.Infrastructure.TaskQueue.SignupEmailVerifyTask;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace FileProcessing.Tests.Integration.Support;

public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _databaseConnection = new("Data Source=:memory:");

    public TestWebApplicationFactory()
    {
        _databaseConnection.Open();
    }

    public IEmailVerifyQueue EmailVerifyQueue => Services.GetRequiredService<IEmailVerifyQueue>();

    public async Task InitializeDatabaseAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Data Source=integration-tests",
                ["Jwt:issuer"] = "file-processing-tests",
                ["Jwt:audience"] = "file-processing-tests-client",
                ["Jwt:key"] = "test-only-signing-key-that-is-at-least-64-characters-long-1234567890",
                ["EmailUrls:VerificationUrl"] = "http://localhost/api/v1/auth/verify-email"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_databaseConnection));

            services.RemoveAll<IHostedService>();
            services.RemoveAll<IAppLogoProvider>();
            services.AddSingleton<IAppLogoProvider, TestAppLogoProvider>();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _databaseConnection.Dispose();
        }
    }

    private sealed class TestAppLogoProvider : IAppLogoProvider
    {
        private static readonly byte[] Logo = [1, 2, 3];

        public Task<byte[]> GetAppLightLogoAsync() => Task.FromResult(Logo);

        public Task<byte[]> GetAppDarkLogoAsync() => Task.FromResult(Logo);
    }
}