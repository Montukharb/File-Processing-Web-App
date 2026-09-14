using FileProcessing.Core.Email.Models;

namespace FileProcessing.Core.Email.Abstraction.EmailTemplateAbstraction
{
    public interface ISignUpTemplateService
    {
        public Task<string> RenderSignUpTemplateAsync(IEmailModel model);
    }

    public interface IAppLogoProvider
    {
        public Task<byte[]> GetAppLightLogoAsync();
        public Task<byte[]> GetAppDarkLogoAsync();
    }
}
