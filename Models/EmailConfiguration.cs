namespace PharmaDistiPro.Models
{
    public class EmailConfiguration
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public bool UseSSL { get; set; }
        public bool UseStartTls { get; set; }
    }
}
