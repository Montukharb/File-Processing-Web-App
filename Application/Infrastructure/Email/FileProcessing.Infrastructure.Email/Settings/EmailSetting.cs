namespace FileProcessing.Infrastructure.Email.Settings
{
    public class EmailSetting
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool UseSSL { get; set; }
    }
}
