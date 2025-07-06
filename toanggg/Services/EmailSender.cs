using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using toanggg.Services;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var smtpHost = _configuration["Email:Smtp:Host"];
        var smtpPort = int.Parse(_configuration["Email:Smtp:Port"]);
        var smtpUser = _configuration["Email:Smtp:Username"];
        var smtpPass = _configuration["Email:Smtp:Password"];
        var enableSsl = bool.Parse(_configuration["Email:Smtp:EnableSsl"]);

        using (var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = enableSsl
        })
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUser),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
        await Task.Delay(100); // Giả lập việc gửi email không đồng bộ
        Console.WriteLine($"Email sent to: {email}. Subject: {subject}. Message: {htmlMessage}");
    }
}
