using FileProcessing.Infrastructure.Email.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessing.Infrastructure.Email.Models
{
    public class UserSignUpModel : IEmailModel
    {
        public required string UserName { get; set; }
        public required string Subject { get; set; }
        public required string To { get; set; }
        public required string Message { get; set; }
        public required string ActionText { get; set; }
        public required string ActionUrl { get; set; }
        public required string TextBody { get; set; }
        public string? HtmlBody { get; set; }
        public required string ContactUsPageUrl { get; set; }
        public required string LogoBaseLightString { get; set; } = string.Empty;
        public required string LogoBaseDarkString { get; set; } = string.Empty;
        public required string HelpUrl { get; set; }
        public required string Report_Link { get; set; }
        public required string Twitter_Link { get; set; }
        public required string LinkedIn_Link { get; set; }
        public required string GitHub_or_Website_Link { get; set; }
    }
}
