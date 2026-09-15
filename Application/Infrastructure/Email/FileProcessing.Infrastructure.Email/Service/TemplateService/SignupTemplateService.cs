using FileProcessing.Infrastructure.Email.Abstraction;
using FileProcessing.Infrastructure.Email.Abstraction.EmailTemplateAbstraction;
using FileProcessing.Infrastructure.Email.Models;
using Razor.Templating.Core;

namespace FileProcessing.Core.Email.Service.TemplateService
{
    public sealed class SignupTemplateService : ISignUpTemplateService
    {
        public async Task<string> RenderSignUpTemplateAsync(IEmailModel model)
        {
            //var templatePath = Path.Combine(Directory.GetCurrentDirectory(),"Templates","UserSignupTemplate.cshtml/");
            var templatePath = Path.Combine("Templates/EmailVerifyTemplate.cshtml");
            return await RazorTemplateEngine.RenderAsync(templatePath, model);
        }
    }
}
