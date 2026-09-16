using FileProcessing.Infrastructure.TaskQueue.SignupEmailVerifyTask;
using Microsoft.Extensions.DependencyInjection;

namespace FileProcessing.Infrastructure.TaskQueue.Composition
{
     public static class DependencyInjection
    {
        public static IServiceCollection EmailVerifyConfiguration(this IServiceCollection service)
        {
            service.AddSingleton<IEmailVerifyQueue, EmailVerifyQueue>();
            return service;
        }
    }
}
