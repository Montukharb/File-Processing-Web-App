using FileProcessing.Infrastructure.Email.Abstraction;
using FileProcessing.Infrastructure.TaskQueue.SignupEmailVerifyTask;
using Microsoft.Extensions.Logging.Abstractions;

namespace FileProcessing.Tests.Unit.Infrastructure.TaskQueue;

public sealed class EmailVerifyQueueTests
{
    [Fact]
    public async Task AddJob_ThenConsumeJob_ReturnsQueuedEmail()
    {
        var queue = new EmailVerifyQueue(NullLogger<EmailVerifyQueue>.Instance);
        var email = new TestEmailModel();

        var added = await queue.AddJob(email);
        var consumed = await queue.ConsumeJob();

        Assert.True(added);
        Assert.Same(email, consumed);
    }

    private sealed class TestEmailModel : IEmailModel
    {
        public string UserName { get; set; } = "Test User";
        public string Subject { get; set; } = "Test subject";
        public string To { get; set; } = "test@example.com";
        public string Message { get; set; } = string.Empty;
        public string ActionText { get; set; } = string.Empty;
        public string ActionUrl { get; set; } = string.Empty;
        public string TextBody { get; set; } = string.Empty;
        public string? HtmlBody { get; set; }
        public string LogoBaseLightString { get; set; } = string.Empty;
        public string LogoBaseDarkString { get; set; } = string.Empty;
        public string ContactUsPageUrl { get; set; } = string.Empty;
        public string HelpUrl { get; set; } = string.Empty;
        public string Report_Link { get; set; } = string.Empty;
        public string Twitter_Link { get; set; } = string.Empty;
        public string LinkedIn_Link { get; set; } = string.Empty;
        public string GitHub_or_Website_Link { get; set; } = string.Empty;
    }
}