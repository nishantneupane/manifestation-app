using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ManifestationApp.Data;
using ManifestationApp.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ManifestationApp.Pages.Goals
{
    [Authorize]
    public class HistoryModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;

        public HistoryModel(UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public List<ManifestationRecord> Records { get; set; }

        public async Task OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);
            Records = await _db.ManifestationRecords
                .Include(r => r.UserGoal)
                .Where(r => r.UserGoal.UserId == userId)
                .OrderByDescending(r => r.SentOn)
                .ToListAsync();
        }
    }
}
