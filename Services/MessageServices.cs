using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;

namespace ItsyBits.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link https://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            try {
                MimeMessage mail = new MimeMessage();
                mail.From.Add(new MailboxAddress("ItsyBits", "admin@itsybits.world"));
                mail.Subject = subject;
                mail.Body = new TextPart("html") {
                    Text = message
                };
                using (SmtpClient client = new SmtpClient()) {
                    client.Connect("127.0.0.1", 25, false);
                    client.Send(mail);
                    client.Disconnect(true);
                }
            }
            catch (Exception e) {
                throw;
            }
            return Task.FromResult(0);
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
