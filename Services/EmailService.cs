using System.Net;
using System.Net.Mail;

namespace QAD_User_Review.Services
{
    public interface IEmailService
    {
        void SendEmail(string toEmail, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            var fromMail = _configuration.GetValue<string>("Email:From") ?? "qad.userreview@spsx.com";
            var smtpHost = _configuration.GetValue<string>("Email:SmtpHost") ?? "relay.spsx.com";
            var smtpPort = _configuration.GetValue<int>("Email:SmtpPort", 25);

            var message = new MailMessage
            {
                From = new MailAddress(fromMail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(toEmail));

            using var smtpClient = new SmtpClient(smtpHost)
            {
                Port = smtpPort,
                EnableSsl = false,
            };
            smtpClient.Send(message);
        }
    }
}
