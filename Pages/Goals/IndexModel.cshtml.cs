// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.RazorPages;
// using Microsoft.AspNetCore.Identity;
// using ManifestationApp.Data;
// using ManifestationApp.Models;
// using System.Threading.Tasks;
// using System.Linq;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.AspNetCore.Authorization;

// namespace ManifestationApp.Pages.Goals
// {
//     [Authorize]
//     public class IndexModel : PageModel
//     {
//         private readonly UserManager<IdentityUser> _userManager;
//         private readonly ApplicationDbContext _db;

//         public IndexModel(UserManager<IdentityUser> userManager, ApplicationDbContext db)
//         {
//             _userManager = userManager;
//             _db = db;
//         }

//         [BindProperty]
//         public string GoalDescription { get; set; }
//         [BindProperty]
//         public Frequency Frequency { get; set; }

//         public async Task OnGetAsync()
//         {
//             var userId = _userManager.GetUserId(User);
//             var goal = await _db.UserGoals.FirstOrDefaultAsync(g => g.UserId == userId);
//             if (goal != null)
//             {
//                 GoalDescription = goal.GoalDescription;
//                 Frequency = goal.ManifestationFrequency;
//             }
//         }

//         public async Task<IActionResult> OnPostAsync()
//         {
//             var userId = _userManager.GetUserId(User);
//             var goal = await _db.UserGoals.FirstOrDefaultAsync(g => g.UserId == userId);

//             if (goal == null)
//             {
//                 goal = new UserGoal
//                 {
//                     UserId = userId,
//                     GoalDescription = GoalDescription,
//                     ManifestationFrequency = Frequency,
//                     LastSent = System.DateTime.UtcNow.AddDays(-((int)Frequency)) // so they get one soon
//                 };
//                 _db.UserGoals.Add(goal);
//             }
//             else
//             {
//                 goal.GoalDescription = GoalDescription;
//                 goal.ManifestationFrequency = Frequency;
//             }

//             await _db.SaveChangesAsync();
//             return RedirectToPage();
//         }
//     }
// }

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ManifestationApp.Data;
using ManifestationApp.Models;
using ManifestationApp.Services; // <-- For IManifestationService
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

namespace ManifestationApp.Pages.Goals
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IManifestationService _manifestationService;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            ApplicationDbContext db,
            IManifestationService manifestationService) // <-- Injected
        {
            _userManager = userManager;
            _db = db;
            _manifestationService = manifestationService;
        }

        [BindProperty]
        public string GoalDescription { get; set; }

        [BindProperty]
        public Frequency Frequency { get; set; }

        // This property holds the immediate affirmation text
        public string Affirmation { get; set; }

        public async Task OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);
            var goal = await _db.UserGoals.FirstOrDefaultAsync(g => g.UserId == userId);

            if (goal != null)
            {
                GoalDescription = goal.GoalDescription;
                Frequency = goal.ManifestationFrequency;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = _userManager.GetUserId(User);
            var goal = await _db.UserGoals.FirstOrDefaultAsync(g => g.UserId == userId);

            if (goal == null)
            {
                goal = new UserGoal
                {
                    UserId = userId,
                    GoalDescription = GoalDescription,
                    ManifestationFrequency = Frequency,
                    LastSent = DateTime.UtcNow.AddDays(-((int)Frequency))
                };
                _db.UserGoals.Add(goal);
            }
            else
            {
                goal.GoalDescription = GoalDescription;
                goal.ManifestationFrequency = Frequency;
            }

            await _db.SaveChangesAsync();

            // Immediately generate the affirmation
            Affirmation = await _manifestationService.GenerateManifestationAsync(goal);

            // Instead of Redirecting, just return the same page
            // so we can display the new Affirmation
            return Page();
        }
    }
}
