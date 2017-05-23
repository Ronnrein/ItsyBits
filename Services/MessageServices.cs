using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace ItsyBits.Services {
    public class AuthMessageSender : IEmailSender {

        public Task SendEmailAsync(string email, string subject, string message) {

            // Prepare mail object
            MimeMessage mail = new MimeMessage();
            mail.From.Add(new MailboxAddress("ItsyBits", "admin@itsybits.world"));
            mail.To.Add(new MailboxAddress(email, email));
            mail.Subject = subject;
            mail.Body = new TextPart("html") { Text = message.Replace(Environment.NewLine, "<br />") };
            return SendEmailAsync(mail);
        }

        public Task SendEmailAsync(MimeMessage mail) {

            // Try sending the email
            try {
                using (SmtpClient client = new SmtpClient()) {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect("127.0.0.1", 25, false);
                    client.Send(mail);
                    client.Disconnect(true);
                }
            }

            // If sending failed, write email to file
            catch (SocketException e) {
                Console.WriteLine("Cannot send emails: " + e.Message);
                Directory.CreateDirectory(@"./bin/mails");
                mail.WriteTo($@"./bin/mails/{DateTime.Now.Ticks}.eml");
            }
            return Task.FromResult(0);
        }

    }
}
