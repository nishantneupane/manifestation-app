using System;

namespace ManifestationApp.Models
{
    public enum Frequency
    {
        Daily = 1,
        Weekly = 7
    }

    public class UserGoal
    {
        public int Id { get; set; }
        public string UserId { get; set; } // FK to IdentityUser
        public string GoalDescription { get; set; }
        public Frequency ManifestationFrequency { get; set; }
        public DateTime LastSent { get; set; }
    }
}
