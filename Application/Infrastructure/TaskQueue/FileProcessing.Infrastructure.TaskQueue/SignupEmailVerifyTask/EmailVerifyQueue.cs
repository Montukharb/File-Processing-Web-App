using FileProcessing.Infrastructure.Email.Abstraction;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace FileProcessing.Infrastructure.TaskQueue.SignupEmailVerifyTask
{
    public class EmailVerifyQueue(ILogger<EmailVerifyQueue> logger) : IEmailVerifyQueue
    {
        public Channel<IEmailModel> CHANNEL { get; } = Channel.CreateUnbounded<IEmailModel>();

        public async Task<bool> AddJob(IEmailModel item)
        {
            try
            {
                await CHANNEL.Writer.WriteAsync(item);
                logger.LogInformation("verify email job add successfully");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError("verify add job failed: {AddJob Exception}", ex);
                throw;
            }
        }

        public async Task<IEmailModel> ConsumeJob()
        {
            try
            {
                return await CHANNEL.Reader.ReadAsync();
            }
            catch (Exception ex)
            {
                logger.LogError("Can't email verify consumed job: {Message}", ex.Message);
                throw;
            }
        }
    }
}
