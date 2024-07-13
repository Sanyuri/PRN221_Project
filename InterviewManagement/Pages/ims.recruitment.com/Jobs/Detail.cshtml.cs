using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;
using Microsoft.AspNetCore.Authorization;

namespace InterviewManagement.Pages.Jobs
{
    [Authorize(Policy = "Job")]
    public class DetailModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;

        public DetailModel(InterviewManagement.Models.InterviewManagementContext context)
        {
            _context = context;
        }

        public Job Job { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null || _context.Schedule == null)
            {
                return NotFound();
            }

            var job = await _context.Job
                .Include(j => j.Skills)
                .Include(j => j.Benefits)
                .Include(j => j.Levels)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }
            else
            {
                Job = job;
            }
            return Page();
        }
    }
}
