using System.Net.Mail;
using System.Net;

namespace RapPhim3.Services
{
    public class EmailService
    {
        private const string EmailSender = "thaiminh1808@gmail.com"; // Thay bằng email của bạn
        private const string AppPassword = "losqhtikdazodaxg"; // Thay bằng App Password

        public async Task SendEmail(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(EmailSender, AppPassword),
                EnableSsl = true,
            };

            using (var message = new MailMessage(EmailSender, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                await smtpClient.SendMailAsync(message);
            }
        }
    }
}
