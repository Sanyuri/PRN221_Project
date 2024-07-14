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
    public class EditModel : PageModel
    {
        private readonly InterviewManagementContext _context;

        public EditModel(InterviewManagementContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Job Job { get; set; } = default!;

        [BindProperty]
        public IList<int> SelectedSkillIds { get; set; } = new List<int>();
        [BindProperty]
        public IList<long> SelectedBenefitIds { get; set; } = new List<long>();
        [BindProperty]
        public IList<int> SelectedLevelIds { get; set; } = new List<int>();

        public IList<Skill> Skills { get; set; } = new List<Skill>();
        public IList<Benefit> Benefits { get; set; } = new List<Benefit>();
        public IList<Level> Levels { get; set; } = new List<Level>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Job == null)
            {
                return NotFound();
            }

            await LoadDataAsync();

            var job = await _context.Job.Include(j => j.Skills)
                                         .Include(j => j.Benefits)
                                         .Include(j => j.Levels)
                                         .FirstOrDefaultAsync(m => m.Id == id);

            if (job == null)
            {
                return NotFound();
            }
            Job = job;
            SelectedSkillIds = job.Skills.Select(s => s.Id).ToList();
            SelectedBenefitIds = job.Benefits.Select(b => b.Id).ToList();
            SelectedLevelIds = job.Levels.Select(l => l.Id).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                return Page();
            }

            if (Job.EndDate <= Job.StartDate)
            {
                ModelState.AddModelError(string.Empty, "End date must be greater than start date.");
                await LoadDataAsync();
                return Page();
            }

            if (Job.SalaryMax <= Job.SalaryMin)
            {
                ModelState.AddModelError(string.Empty, "Max salary must be greater than min salary.");
                await LoadDataAsync();
                return Page();
            }

            var jobToUpdate = await _context.Job.Include(j => j.Skills)
                                                .Include(j => j.Benefits)
                                                .Include(j => j.Levels)
                                                .FirstOrDefaultAsync(m => m.Id == Job.Id);

            if (jobToUpdate == null)
            {
                return NotFound();
            }

            jobToUpdate.JobName = Job.JobName;
            jobToUpdate.StartDate = Job.StartDate;
            jobToUpdate.EndDate = Job.EndDate;
            jobToUpdate.SalaryMin = Job.SalaryMin;
            jobToUpdate.SalaryMax = Job.SalaryMax;
            jobToUpdate.WorkingAddress = Job.WorkingAddress;
            jobToUpdate.Status = Job.Status;
            jobToUpdate.IsDeleted = Job.IsDeleted;
            jobToUpdate.ModifiedBy = Job.ModifiedBy;
            jobToUpdate.Description = Job.Description;

            foreach (var skillId in SelectedSkillIds)
            {
                var skill = await _context.Skill.FindAsync(skillId);
                if (skill != null)
                {
                    if (!jobToUpdate.Skills.Contains(skill))
                    {
                        jobToUpdate.Skills.Add(skill);
                    }
                }
            }

            foreach (var benefitId in SelectedBenefitIds)
            {
                var benefit = await _context.Benefit.FindAsync(benefitId);
                if (benefit != null)
                {
                    if (!jobToUpdate.Benefits.Contains(benefit))
                    {
                        jobToUpdate.Benefits.Add(benefit);
                    }
                }
            }

            foreach (var levelId in SelectedLevelIds)
            {
                var level = await _context.Level.FindAsync(levelId);
                if (level != null)
                {
                    if (!jobToUpdate.Levels.Contains(level))
                    {
                        jobToUpdate.Levels.Add(level);
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobExists(Job.Id))
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

        private bool JobExists(int id)
        {
            return (_context.Job?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private async Task LoadDataAsync()
        {
            Skills = await _context.Skill.ToListAsync();
            Benefits = await _context.Benefit.ToListAsync();
            Levels = await _context.Level.ToListAsync();
        }
    }
}
