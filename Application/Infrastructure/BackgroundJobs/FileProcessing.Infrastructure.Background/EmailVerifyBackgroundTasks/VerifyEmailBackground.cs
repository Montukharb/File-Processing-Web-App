using FileProcessing.Infrastructure.Email.Abstraction;
using FileProcessing.Infrastructure.TaskQueue.SignupEmailVerifyTask;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FileProcessing.Infrastructure.Background.EmailVerifyBackgroundTasks;

public class VerifyEmailBackground(IServiceScopeFactory scopeFactory, IEmailVerifyQueue jobQueue, ILogger<VerifyEmailBackground> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var emailModel = await jobQueue.ConsumeJob();
                using var scope = scopeFactory.CreateAsyncScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                await emailService.SendEmailAsync(emailModel);

                logger.LogInformation("Verification email processed.");
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError("Exception occurred during verify email. {exception}", ex);
            }
        }
    }
}