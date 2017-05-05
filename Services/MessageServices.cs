using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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
            MimeMessage mail = new MimeMessage();
            mail.From.Add(new MailboxAddress("ItsyBits", "admin@itsybits.world"));
            mail.To.Add(new MailboxAddress(email, email));
            mail.Subject = subject;
            mail.Body = new TextPart("html") {
                Text = message
            };
            try {
                using (SmtpClient client = new SmtpClient()) {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect("127.0.0.1", 25, false);
                    client.Send(mail);
                    client.Disconnect(true);
                }
            }
            catch (SocketException e) {
                Console.WriteLine("Cannot send emails: "+e.Message);
                Directory.CreateDirectory(@"./bin/mails");
                Console.WriteLine($@"./bin/mails/{DateTime.Now.Ticks}.eml");
                mail.WriteTo($@"./bin/mails/{DateTime.Now.Ticks}.eml");
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
