
namespace FileProcessing.Infrastructure.Email.Abstraction
{
    public interface IEmailService
    {
        Task SendEmailAsync<TEmailModel>(TEmailModel model) where TEmailModel : IEmailModel;
    }
}
