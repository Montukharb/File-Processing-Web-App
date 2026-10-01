using FileProcessing.Infrastructure.Email.Abstraction;
using FileProcessing.Infrastructure.Email.Settings;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using FileProcessing.Infrastructure.Email.Abstraction.EmailTemplateAbstraction;


namespace FileProcessing.Core.Email.Service
{
    public class EmailService : IEmailService
    {
    private readonly EmailSetting _emailSettings;
        private readonly ISignUpTemplateService _signUpTemplateService;
     
     public EmailService(IOptions<EmailSetting> emailSettings, ISignUpTemplateService SignupTemplateService)
        {
            _emailSettings = emailSettings.Value;
            _signUpTemplateService = SignupTemplateService;

        } 
        
        public async Task SendEmailAsync<TEmailModel>(TEmailModel model) where TEmailModel : IEmailModel
        {
            var message = new MimeMessage();

            message.From.Add(CreateMailboxAddress(
                _emailSettings.DisplayName,
                _emailSettings.Email,
                "EmailSettings:Email"
                ));

            message.To.Add(CreateMailboxAddress(
                model.UserName,
                model.To,
                "email model recipient (To)"
                ));

            message.Subject = model.Subject;
            var bodyBuilder = await EmailMessageBodyBuilder.CreateAsync(model, _signUpTemplateService);
            message.Body = bodyBuilder.ToMessageBody();

            using var smptClient = new SmtpClient();

                await smptClient.ConnectAsync(
                    _emailSettings.Host,
                    _emailSettings.Port,
                    SecureSocketOptions.StartTls
                    );

            await smptClient.AuthenticateAsync(
                _emailSettings.UserName,
                _emailSettings.Password
                );

            await smptClient.SendAsync(message);

            await smptClient.DisconnectAsync(true);
        }

        private static MailboxAddress CreateMailboxAddress(string? displayName, string? address, string settingName)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new InvalidOperationException($"{settingName} is required and must be a valid email address.");
            }

            var normalizedAddress = address.Trim();
            if (!System.Net.Mail.MailAddress.TryCreate(normalizedAddress, out var parsedAddress) ||
                !string.Equals(parsedAddress.Address, normalizedAddress, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"{settingName} must be a valid email address.");
            }

            try
            {
                return new MailboxAddress(displayName ?? string.Empty, normalizedAddress);
            }
            catch (ParseException exception)
            {
                throw new InvalidOperationException(
                    $"{settingName} must be a valid email address. Check the configured sender and recipient values.",
                    exception);
            }
        }
    }
}
