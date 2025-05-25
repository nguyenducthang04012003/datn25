using MimeKit;
using PharmaDistiPro.Models;
using PharmaDistiPro.Services.Interface;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace PharmaDistiPro.Services.Impl
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _smtpSettings;

        public EmailService(EmailConfiguration smtpSettings)
        {
            _smtpSettings = smtpSettings;
        }


        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;
                message.Body = new TextPart("plain") { Text = body };

                using (var client = new SmtpClient())
                {
                    Console.WriteLine($"Connecting to {_smtpSettings.SmtpServer} on port {_smtpSettings.Port}...");

                    await client.ConnectAsync(_smtpSettings.SmtpServer, _smtpSettings.Port, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);

                    Console.WriteLine("✅ Email sent successfully.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                Console.WriteLine($"SMTP Sender: {_smtpSettings.SenderEmail}");
                Console.WriteLine($"SMTP Server: {_smtpSettings.SmtpServer}");
                Console.WriteLine($"To Email: {toEmail}");

                return false;
            }
        }

    }
}
