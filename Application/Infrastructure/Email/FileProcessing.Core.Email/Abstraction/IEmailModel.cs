
using FileProcessing.Core.Email.Models;

namespace FileProcessing.Core.Email.Abstraction
{
    public interface IEmailModel
    {
        public string UserName { get; set; }
        public string Subject { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
        public string? ActionText { get; set; }
        public string? ActionUrl { get; set; }
        public string TextBody { get; set; }
        public string? HtmlBody { get; set; }
        public string LogoBaseLightString { get; set; }
        public string LogoBaseDarkString { get; set; }
    }
}