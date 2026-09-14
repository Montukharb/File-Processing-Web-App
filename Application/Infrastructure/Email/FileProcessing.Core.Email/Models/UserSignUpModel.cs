using FileProcessing.Core.Email.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessing.Core.Email.Models
{
    public class UserSignUpModel : IEmailModel
    {
        public required string UserName { get; set; }
        public required string Subject { get; set; }
        public required string To { get; set; }
        public required string Message { get; set; }
        public string? ActionText { get; set; }
        public string? ActionUrl { get; set; }
        public required string TextBody { get; set; }
        public string? HtmlBody { get; set; }
        public required string LogoBaseLightString { get; set; }
        public required string LogoBaseDarkString { get; set; }
    }
}
