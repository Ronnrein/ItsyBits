using System.Threading.Tasks;
using MimeKit;

namespace ItsyBits.Services {
    public interface IEmailSender {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendEmailAsync(MimeMessage email);
    }
}
