using FileProcessing.Infrastructure.Email.Abstraction;
using FileProcessing.Infrastructure.Email.Abstraction.EmailTemplateAbstraction;
using MimeKit;

namespace FileProcessing.Core.Email.Service;

public static class EmailMessageBodyBuilder
{
    public static async Task<BodyBuilder> CreateAsync(IEmailModel model, ISignUpTemplateService templateService)
    {
        var lightLogo = CreateInlineImage(model.LogoBaseLightString, "logo-light.png", "email-logo-light");
        var darkLogo = CreateInlineImage(model.LogoBaseDarkString, "logo-dark.png", "email-logo-dark");

        model.LogoBaseLightString = $"cid:{lightLogo.ContentId}";
        model.LogoBaseDarkString = $"cid:{darkLogo.ContentId}";

        var bodyBuilder = new BodyBuilder
        {
            TextBody = model.TextBody,
            HtmlBody = await templateService.RenderSignUpTemplateAsync(model)
        };

        bodyBuilder.LinkedResources.Add(lightLogo);
        bodyBuilder.LinkedResources.Add(darkLogo);
        return bodyBuilder;
    }

    private static MimePart CreateInlineImage(string dataUri, string fileName, string contentId)
    {
        var separatorIndex = dataUri.IndexOf(',');
        if (!dataUri.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase) ||
            separatorIndex < 0 ||
            !dataUri[..separatorIndex].EndsWith(";base64", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"The email image '{fileName}' must be a base64 image data URI.");
        }

        var mediaType = dataUri["data:".Length..separatorIndex].Split(';', 2)[0];
        byte[] imageBytes;
        try
        {
            imageBytes = Convert.FromBase64String(dataUri[(separatorIndex + 1)..]);
        }
        catch (FormatException exception)
        {
            throw new InvalidOperationException($"The email image '{fileName}' contains invalid base64 data.", exception);
        }

        return new MimePart(ContentType.Parse(mediaType))
        {
            Content = new MimeContent(new MemoryStream(imageBytes)),
            ContentId = contentId,
            ContentDisposition = new ContentDisposition(ContentDisposition.Inline),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = fileName
        };
    }
}