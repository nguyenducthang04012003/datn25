namespace PharmaDistiPro.Services.Interface
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body);

    }
}
