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

            message.From.Add(new MailboxAddress(
                _emailSettings.DisplayName,
                _emailSettings.Email
                ));

            message.To.Add(new MailboxAddress(
                model.UserName,
                model.To
                ));

            message.Subject = model.Subject;
            var bodyBuilder = new BodyBuilder()
            {
                TextBody = model.TextBody,
                HtmlBody = await _signUpTemplateService.RenderSignUpTemplateAsync(model)
            };
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
    }
}
