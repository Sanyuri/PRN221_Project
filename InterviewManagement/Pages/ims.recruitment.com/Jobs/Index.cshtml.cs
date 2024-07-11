using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;
using InterviewManagement.Utils;
using Microsoft.AspNetCore.Authorization;

namespace InterviewManagement.Pages.Jobs
{
    [Authorize(Policy = "Job")]
    public class IndexModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;

        public IndexModel(InterviewManagement.Models.InterviewManagementContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; }

        public PaginatedList<Job> Job { get; set; }

        public async Task OnGetAsync(string sortOrder, string searchString, string status, int? pageNumber)
        {
            SortOrder = sortOrder;
            Keyword = searchString;
            StatusFilter = status;

            ViewData["CurrentSort"] = sortOrder;
            ViewData["Keyword"] = searchString;
            ViewData["StatusFilter"] = status;

            await UpdateJobStatusesAsync();

            IQueryable<Job> jobQuery = _context.Job
                .Include(j => j.Skills)
                .Include(j => j.Levels)
                .Where(j => j.IsDeleted == false);

            // Search
            if (!String.IsNullOrEmpty(Keyword))
            {
                jobQuery = jobQuery.Where(j => j.JobName.Contains(Keyword));
            }

            if (!String.IsNullOrEmpty(StatusFilter))
            {
                jobQuery = jobQuery.Where(j => j.Status.Contains(StatusFilter));
            }

            // Sorting
            switch (SortOrder)
            {
                default:
                    jobQuery = jobQuery.OrderBy(s => s.Status == "Open" ? 1 : s.Status == "Draft" ? 2 : 3)
                                       .ThenByDescending(s => s.StartDate);
                    break;
            }

            int pageSize = 10;
            Job = await PaginatedList<Job>.CreateAsync(jobQuery.AsNoTracking(), pageNumber ?? 1, pageSize);
        }

        private async Task UpdateJobStatusesAsync()
        {
            var jobs = await _context.Job.ToListAsync();
            var currentDate = DateTime.UtcNow;

            foreach (var job in jobs)
            {
                if (job.StartDate <= currentDate && job.EndDate >= currentDate)
                {
                    job.Status = "Open";
                }
                else if (currentDate > job.EndDate)
                {
                    job.Status = "Close";
                }
            }

            await _context.SaveChangesAsync();
        }

        public IActionResult OnPostDelete(int jobId)
        {
            var job = _context.Job.Find(jobId);

            if (job != null)
            {
                job.IsDeleted = true;
                _context.SaveChanges();
            }

            return RedirectToPage("Index");
        }
    
    }
}
