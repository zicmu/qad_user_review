using System;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;

namespace QAD_User_Review.Services
{
    public class EmailService
    {
        public void SendEmail(string toEmail, string emailSubject, string emailBody)
        {
            string fromMail = "qad.userreview@spsx.com";
            string fromPassword = "";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = emailSubject;
            message.To.Add(new MailAddress(toEmail));
            message.Body = emailBody;
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("relay.spsx.com")
            {
                Port = 25,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = false,
            };
            smtpClient.Send(message);
        }
    }
}
