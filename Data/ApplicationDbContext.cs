using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ManifestationApp.Models;

namespace ManifestationApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserGoal> UserGoals { get; set; }
        public DbSet<ManifestationRecord> ManifestationRecords { get; set; }
    }
}
