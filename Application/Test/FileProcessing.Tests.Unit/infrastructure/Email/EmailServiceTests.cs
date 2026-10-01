using FileProcessing.Core.Email.Service;
using FileProcessing.Infrastructure.Email.Abstraction;
using FileProcessing.Infrastructure.Email.Abstraction.EmailTemplateAbstraction;
using FileProcessing.Infrastructure.Email.Models;
using FileProcessing.Infrastructure.Email.Settings;
using Microsoft.Extensions.Options;

namespace FileProcessing.Tests.Unit.Infrastructure.Email;

public sealed class EmailServiceTests
{
    [Fact]
    public async Task SendEmailAsync_InvalidSender_ThrowsConfigurationError()
    {
        var service = CreateService("not-a-mailbox");

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.SendEmailAsync(CreateEmailModel()));

        Assert.Contains("EmailSettings:Email", exception.Message);
    }

    [Fact]
    public async Task SendEmailAsync_MissingRecipient_ThrowsRecipientError()
    {
        var service = CreateService("sender@example.com");
        var email = CreateEmailModel();
        email.To = string.Empty;

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.SendEmailAsync(email));

        Assert.Contains("recipient", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static EmailService CreateService(string senderAddress) => new(
        Options.Create(new EmailSetting
        {
            DisplayName = "Test Sender",
            Email = senderAddress
        }),
        new TestTemplateService());

    private static UserSignUpModel CreateEmailModel() => new()
    {
        UserName = "Test User",
        Subject = "Test subject",
        To = "recipient@example.com",
        Message = "Test message",
        ActionText = "Verify",
        ActionUrl = "https://example.com/verify",
        TextBody = "Test body",
        ContactUsPageUrl = "https://example.com/contact",
        LogoBaseLightString = string.Empty,
        LogoBaseDarkString = string.Empty,
        HelpUrl = "https://example.com/help",
        Report_Link = "https://example.com/report",
        Twitter_Link = string.Empty,
        LinkedIn_Link = string.Empty,
        GitHub_or_Website_Link = "https://example.com"
    };

    private sealed class TestTemplateService : ISignUpTemplateService
    {
        public Task<string> RenderSignUpTemplateAsync(IEmailModel model) => Task.FromResult("<p>Test</p>");
    }
}