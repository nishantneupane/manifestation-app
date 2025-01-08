using System;

namespace ManifestationApp.Models
{
    public class ManifestationRecord
    {
        public int Id { get; set; }
        public int UserGoalId { get; set; }
        public UserGoal UserGoal { get; set; }
        public string Message { get; set; }
        public DateTime SentOn { get; set; }
    }
}
