using System.Threading.Tasks;

namespace ManifestationApp.Services
{
    public interface IEmailService
    {
        Task SendManifestationEmail(string toEmail, string subject, string message);
    }
}
