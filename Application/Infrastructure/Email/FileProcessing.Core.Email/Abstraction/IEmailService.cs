using FileProcessing.Core.Email.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessing.Core.Email.Abstraction
{
    public interface IEmailService
    {
        Task SendEmailAsync<TEmailModel>(TEmailModel model) where TEmailModel : IEmailModel;
    }
}
