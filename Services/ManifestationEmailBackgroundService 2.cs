using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ManifestationApp.Data;
using ManifestationApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ManifestationApp.Services
{
    public class ManifestationEmailBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        public ManifestationEmailBackgroundService(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var manifestationService = scope.ServiceProvider.GetRequiredService<IManifestationService>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    // Get due goals
                    var now = DateTime.UtcNow;
                    var goals = await dbContext.UserGoals
                        .ToListAsync(stoppingToken);

                    // For each goal, check if due
                    foreach (var goal in goals)
                    {
                        int daysSinceLastSent = (int)(now - goal.LastSent).TotalDays;
                        if (daysSinceLastSent >= (int)goal.ManifestationFrequency)
                        {
                            // Generate manifestation
                            var affirmation = await manifestationService.GenerateManifestationAsync(goal);

                            // We need user's email. Since we used IdentityUser, let's fetch it:
                            // We'll do a separate query because goal.UserId is a string (User's Id)
                            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == goal.UserId);
                            if (user != null)
                            {
                                await emailService.SendManifestationEmail(user.Email, "Your Daily Manifestation", affirmation);

                                // Record in history
                                var record = new ManifestationRecord
                                {
                                    UserGoalId = goal.Id,
                                    Message = affirmation,
                                    SentOn = now
                                };
                                dbContext.ManifestationRecords.Add(record);

                                // Update LastSent
                                goal.LastSent = now;
                                await dbContext.SaveChangesAsync(stoppingToken);
                            }
                        }
                    }
                }

                // Wait for 1 hour
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
