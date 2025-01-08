using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using ManifestationApp.Services;

namespace ManifestationApp.Services
{
    public class IdentityEmailSender : IEmailSender
    {
        private readonly IEmailService _emailService;

        public IdentityEmailSender(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Use your IEmailService to send the email.
            return _emailService.SendManifestationEmail(email, subject, htmlMessage);
        }
    }
}
