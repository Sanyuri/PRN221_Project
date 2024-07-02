using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using InterviewManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace InterviewManagement.Pages.Schedules
{
    public class CreateModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;

        public CreateModel(InterviewManagement.Models.InterviewManagementContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Schedule Schedule { get; set; } = new Schedule();
        public IList<Job> Jobs { get; set; } = new List<Job>();
        public IList<Candidate> Candidates { get; set; } = new List<Candidate>();
        public IList<Employee> Employees { get; set; } = new List<Employee>();

        [BindProperty]
        public int SelectedJobId { get; set; } = new int();
        [BindProperty]
        public long SelectedCandidateId { get; set; } = new long();
        [BindProperty]
        public IList<long> SelectedInterviewerIds { get; set; } = new List<long>();

        public async Task<IActionResult> OnGetAsync()
        {
            Jobs = await _context.Job.ToListAsync();
            Candidates = await _context.Candidate.ToListAsync();
            Employees = await _context.Employee.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Jobs = await _context.Job.ToListAsync();
                Candidates = await _context.Candidate.ToListAsync();
                Employees = await _context.Employee.ToListAsync();
                return Page();
            }


            Schedule.Employees = new List<Employee>();
            foreach (var interviewerId in SelectedInterviewerIds)
            {
                var interviewer = await _context.Employee.FindAsync(interviewerId);
                if (interviewer != null)
                {
                    Schedule.Employees.Add(interviewer);
                }
            }

            var candidate = await _context.Candidate.FindAsync(SelectedCandidateId);
            if (candidate != null)
            {
                Schedule.Candidate = candidate;
            }

            var job = await _context.Job.FindAsync(SelectedJobId);
            if (job != null)
            {
                Schedule.Job = job;
            }

            _context.Schedule.Add(Schedule);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
