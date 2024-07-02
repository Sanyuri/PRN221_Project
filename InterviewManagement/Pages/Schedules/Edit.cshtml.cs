using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;

namespace InterviewManagement.Pages.Schedules
{
    public class EditModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;

        public EditModel(InterviewManagement.Models.InterviewManagementContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Schedule Schedule { get; set; } = default!;
        public IList<Job> Jobs { get; set; } = new List<Job>();
        public IList<Candidate> Candidates { get; set; } = new List<Candidate>();
        public IList<Employee> Employees { get; set; } = new List<Employee>();

        [BindProperty]
        public int SelectedJobId { get; set; } = new int();
        [BindProperty]
        public int SelectedCandidateId { get; set; } = new int();
        [BindProperty]
        public IList<int> SelectedInterviewerIds { get; set; } = new List<int>();

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null || _context.Schedule == null)
            {
                return NotFound();
            }

            Jobs = await _context.Job.ToListAsync();
            Candidates = await _context.Candidate.ToListAsync();
            Employees = await _context.Employee.ToListAsync();

            var schedule =  await _context.Schedule
                .Include(j => j.Job)
                .Include(j => j.Candidate)
                .Include(j => j.Employees)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (schedule == null)
            {
                return NotFound();
            }
            Schedule = schedule;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Schedule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

            return RedirectToPage("./Index");
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

            return RedirectToPage("Edit", new { id = scheduleId });
        }

    }
}
