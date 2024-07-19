using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;
using InterviewManagement.Utils;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace InterviewManagement.Pages.Schedules
{
    [Authorize(Policy = "Employee")]

    public class IndexModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;

        public IndexModel(InterviewManagement.Models.InterviewManagementContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string InterviewerFilter { get; set; }

        public PaginatedList<Schedule> Schedule { get; set; }
        public List<Employee> Employees { get; set; }

        public async Task OnGetAsync(string searchString, string status, string interviewer, int? pageNumber)
        {
            long accountId = long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value);
            var sessionRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            Keyword = searchString;
            StatusFilter = status;
            InterviewerFilter = interviewer;

            ViewData["Keyword"] = searchString;
            ViewData["StatusFilter"] = status;
            ViewData["InterviewerFilter"] = interviewer;

            IQueryable<Schedule> scheduleQuery = _context.Schedule
                .Include(j => j.Candidate)
                .Include(j => j.Job)
                .Include(j => j.Employees)
                .Where(j => j.IsDeleted == false);
            Employees = _context.Employee.ToList();

            if (sessionRole.Equals("Interviewer"))
            {
                scheduleQuery = scheduleQuery.Include(e => e.Employees).Where(e => e.Employees.Any(i => i.Id == accountId));
            }

            if (!String.IsNullOrEmpty(Keyword))
            {
                scheduleQuery = scheduleQuery.Where(j => j.ScheduleName.Contains(Keyword));
            }

            if (!String.IsNullOrEmpty(StatusFilter))
            {
                scheduleQuery = scheduleQuery.Where(j => j.Status.Contains(StatusFilter));
            }

            if (!String.IsNullOrEmpty(InterviewerFilter))
            {
                scheduleQuery = scheduleQuery.Where(j => j.Employees.Any(e => e.FullName == InterviewerFilter));
            }

            int pageSize = 10;
            Schedule = await PaginatedList<Schedule>.CreateAsync(scheduleQuery.AsNoTracking(), pageNumber ?? 1, pageSize);
        }

    }
}
