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
    public class CreateModel : PageModel
    {
        private readonly InterviewManagementContext _context;

        public CreateModel(InterviewManagementContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Job Job { get; set; } = new Job();

        [BindProperty]
        public IList<int> SelectedSkillIds { get; set; } = new List<int>();
        [BindProperty]
        public IList<long> SelectedBenefitIds { get; set; } = new List<long>();
        [BindProperty]
        public IList<int> SelectedLevelIds { get; set; } = new List<int>();

        public IList<Skill> Skills { get; set; } = new List<Skill>();
        public IList<Benefit> Benefits { get; set; } = new List<Benefit>();
        public IList<Level> Levels { get; set; } = new List<Level>();


        public async Task<IActionResult> OnGetAsync()
        {
            Skills = await _context.Skill.ToListAsync();
            Benefits = await _context.Benefit.ToListAsync();
            Levels = await _context.Level.ToListAsync();          

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Skills = await _context.Skill.ToListAsync();
                Benefits = await _context.Benefit.ToListAsync();
                Levels = await _context.Level.ToListAsync();
                return Page();
            }

            var currentDate = DateTime.UtcNow;

            if (Job.StartDate < currentDate)
            {
                ModelState.AddModelError(string.Empty, "Start date must be greater than now");
                Skills = await _context.Skill.ToListAsync();
                Benefits = await _context.Benefit.ToListAsync();
                Levels = await _context.Level.ToListAsync();
                return Page();
            }

            if (Job.EndDate <= Job.StartDate)
            {
                ModelState.AddModelError(string.Empty, "End date must be greater than start date.");
                Skills = await _context.Skill.ToListAsync();
                Benefits = await _context.Benefit.ToListAsync();
                Levels = await _context.Level.ToListAsync();
                return Page();
            }

            if (Job.SalaryMax <= Job.SalaryMin)
            {
                ModelState.AddModelError(string.Empty, "Max salary must be greater than min salary.");
                Skills = await _context.Skill.ToListAsync();
                Benefits = await _context.Benefit.ToListAsync();
                Levels = await _context.Level.ToListAsync();
                return Page();
            }

            Job.IsDeleted = false;
            Job.Status = "Draft";           

            Job.Skills = new List<Skill>();
            foreach (var skillId in SelectedSkillIds)
            {
                var skill = await _context.Skill.FindAsync(skillId);
                if (skill != null)
                {
                    Job.Skills.Add(skill);
                }
            }        

            Job.Benefits = new List<Benefit>();
            foreach (var benefitId in SelectedBenefitIds)
            {
                var benefit = await _context.Benefit.FindAsync(benefitId);
                if (benefit != null)
                {
                    Job.Benefits.Add(benefit);
                }
            }

            Job.Levels = new List<Level>();
            foreach (var levelId in SelectedLevelIds)
            {
                var level = await _context.Level.FindAsync(levelId);
                if (level != null)
                {
                    Job.Levels.Add(level);
                }
            }

            

            _context.Job.Add(Job);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

       
    }
}
