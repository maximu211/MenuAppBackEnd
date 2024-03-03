using System.Net;
using System.Net.Mail;
using MenuApp.DAL.Configurations;
using Microsoft.Extensions.Options;

namespace MenuApp.BLL.Utils
{
    public interface IEmailSender
    {
        Task<MailMessage> SendEmail(string receiverMail, string subject, string message);
    }

    public class EmailSender : IEmailSender
    {
        private readonly IOptions<EmailSettings> _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings;
        }

        public async Task<MailMessage> SendEmail(
            string receiverMail,
            string subject,
            string message
        )
        {
            string senderMail = _emailSettings.Value.Email;
            string senderPassword = _emailSettings.Value.Password;

            var mailMessage = new MailMessage(from: senderMail, to: receiverMail, subject, message);

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(senderMail, senderPassword)
            };

            await client.SendMailAsync(mailMessage);

            return mailMessage;
        }
    }
}
