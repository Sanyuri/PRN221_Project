using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;
using Microsoft.AspNetCore.Authorization;

namespace InterviewManagement.Pages.Schedules
{
    [Authorize(Policy = "Employee")]
    public class EditModel : PageModel
    {
        private readonly InterviewManagementContext _context;

        public EditModel(InterviewManagementContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Schedule Schedule { get; set; } = default!;
        public IList<Job> Jobs { get; set; } = new List<Job>();
        public IList<Candidate> Candidates { get; set; } = new List<Candidate>();
        public IList<Employee> Employees { get; set; } = new List<Employee>();

        [BindProperty]
        public int SelectedJobId { get; set; }
        [BindProperty]
        public long SelectedCandidateId { get; set; }
        [BindProperty]
        public IList<long> SelectedInterviewerIds { get; set; } = new List<long>();

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null || _context.Schedule == null)
            {
                return NotFound();
            }

            await LoadDataAsync();

            var schedule = await _context.Schedule
                .Include(s => s.Job)
                .Include(s => s.Candidate)
                .Include(s => s.Employees)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (schedule == null)
            {
                return NotFound();
            }

            Schedule = schedule;
            SelectedJobId = schedule.Job?.Id ?? 0;
            SelectedCandidateId = schedule.Candidate?.Id ?? 0;
            SelectedInterviewerIds = schedule.Employees.Select(e => e.Id).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                return Page();
            }

            var scheduleToUpdate = await _context.Schedule
                .Include(s => s.Job)
                .Include(s => s.Candidate)
                .Include(s => s.Employees)
                .FirstOrDefaultAsync(m => m.Id == Schedule.Id);

            if (scheduleToUpdate == null)
            {
                return NotFound();
            }

            scheduleToUpdate.ScheduleName = Schedule.ScheduleName;
            scheduleToUpdate.ScheduleTime = Schedule.ScheduleTime;
            scheduleToUpdate.Note = Schedule.Note;
            scheduleToUpdate.MeetingURL = Schedule.MeetingURL;
            scheduleToUpdate.ModifiedBy = Schedule.ModifiedBy;
            scheduleToUpdate.Result = Schedule.Result;
            scheduleToUpdate.Location = Schedule.Location;
            scheduleToUpdate.IsDeleted = Schedule.IsDeleted;
            scheduleToUpdate.Status = Schedule.Status;

            if (SelectedJobId != scheduleToUpdate.Job?.Id)
            {
                var job = await _context.Job.FindAsync(SelectedJobId);
                if (job != null)
                {
                    scheduleToUpdate.Job = job;
                }
            }

            if (SelectedCandidateId != scheduleToUpdate.Candidate?.Id)
            {
                var candidate = await _context.Candidate.FindAsync(SelectedCandidateId);
                if (candidate != null)
                {
                    scheduleToUpdate.Candidate = candidate;
                }
            }

            scheduleToUpdate.Employees.Clear();
            foreach (var interviewerId in SelectedInterviewerIds)
            {
                var employee = await _context.Employee.FindAsync(interviewerId);
                if (employee != null)
                {
                    scheduleToUpdate.Employees.Add(employee);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Schedule edited successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScheduleExists(Schedule.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("Details", new { id = Schedule.Id });
        }

        private bool ScheduleExists(long id)
        {
            return (_context.Schedule?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public IActionResult OnPostDelete(long scheduleId)
        {
            var schedule = _context.Schedule.Find(scheduleId);

            if (schedule != null)
            {
                schedule.Status = "Cancelled";
                _context.SaveChanges();
            }

            TempData["SuccessMessage"] = "Schedule have been cancelled";

            return RedirectToPage("Details", new { id = scheduleId });
        }

        private async Task LoadDataAsync()
        {
            Jobs = await _context.Job.Where(j => j.Status == "Open").ToListAsync();
            Candidates = await _context.Candidate.ToListAsync();
            Employees = await _context.Employee.Include(e => e.Role).ToListAsync();
        }
    }
}
