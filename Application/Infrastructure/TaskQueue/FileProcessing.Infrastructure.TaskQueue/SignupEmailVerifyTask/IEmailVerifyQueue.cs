using FileProcessing.Infrastructure.Email.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace FileProcessing.Infrastructure.TaskQueue.SignupEmailVerifyTask
{
    public interface IEmailVerifyQueue
    {
        public Channel<IEmailModel> CHANNEL { get;}
        Task<bool> AddJob(IEmailModel item);
        Task<IEmailModel> ConsumeJob(); 
    }
}
