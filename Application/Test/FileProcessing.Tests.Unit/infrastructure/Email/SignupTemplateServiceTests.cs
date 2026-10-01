using FileProcessing.Core.Email.Service;
using FileProcessing.Core.Email.Service.TemplateService;
using FileProcessing.Infrastructure.Email.Models;

namespace FileProcessing.Tests.Unit.Infrastructure.Email;

public sealed class SignupTemplateServiceTests
{
    [Fact]
    public async Task RenderSignUpTemplateAsync_ReturnsRenderedHtmlDocument()
    {
        var service = new SignupTemplateService();

        var html = await service.RenderSignUpTemplateAsync(new UserSignUpModel
        {
            UserName = "Test User",
            Subject = "Verify email",
            To = "test@example.com",
            Message = "Verify your email",
            ActionText = "Verify",
            ActionUrl = "https://example.com/verify?token=test",
            TextBody = "Verify your email: https://example.com/verify?token=test",
            ContactUsPageUrl = "https://example.com/contact",
            LogoBaseLightString = "data:image/png;base64,dGVzdA==",
            LogoBaseDarkString = "data:image/png;base64,dGVzdA==",
            HelpUrl = "https://example.com/help",
            Report_Link = "https://example.com/report",
            Twitter_Link = "https://example.com/social",
            LinkedIn_Link = "https://example.com/social",
            GitHub_or_Website_Link = "https://example.com"
        });

        Assert.Contains("<!DOCTYPE html>", html, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Welcome to the Family!", html);
        Assert.DoesNotContain("@Model.", html);
    }

    [Fact]
    public async Task EmailBody_UsesInlineLogoPartsInsteadOfEmbeddingBase64InHtml()
    {
        var model = new UserSignUpModel
        {
            UserName = "Test User",
            Subject = "Verify email",
            To = "test@example.com",
            Message = "Verify your email",
            ActionText = "Verify",
            ActionUrl = "https://example.com/verify?token=test",
            TextBody = "Verify your email",
            ContactUsPageUrl = "https://example.com/contact",
            LogoBaseLightString = "data:image/png;base64,dGVzdA==",
            LogoBaseDarkString = "data:image/png;base64,dGVzdA==",
            HelpUrl = "https://example.com/help",
            Report_Link = "https://example.com/report",
            Twitter_Link = "https://example.com/social",
            LinkedIn_Link = "https://example.com/social",
            GitHub_or_Website_Link = "https://example.com"
        };
        var templateService = new SignupTemplateService();

        var body = await EmailMessageBodyBuilder.CreateAsync(model, templateService);

        Assert.Contains("cid:email-logo-light", body.HtmlBody);
        Assert.Contains("cid:email-logo-dark", body.HtmlBody);
        Assert.DoesNotContain("data:image/png;base64", body.HtmlBody);
        Assert.Equal(2, body.LinkedResources.Count);
        Assert.Equal("Verify your email", body.TextBody);
    }
}