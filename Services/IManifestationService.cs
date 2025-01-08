using System.Threading.Tasks;
using ManifestationApp.Models;

namespace ManifestationApp.Services
{
    public interface IManifestationService
    {
        Task<string> GenerateManifestationAsync(UserGoal goal);
    }
}
