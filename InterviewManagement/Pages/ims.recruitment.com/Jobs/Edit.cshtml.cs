using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;

namespace InterviewManagement.Pages.Jobs
{
    public class EditModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;

        public EditModel(InterviewManagement.Models.InterviewManagementContext context)
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

            Skills = await _context.Skill.ToListAsync();
            Benefits = await _context.Benefit.ToListAsync();
            Levels = await _context.Level.ToListAsync();

            var job =  await _context.Job.Include(j => j.Skills)
                                         .Include(j => j.Benefits)
                                         .Include(j => j.Levels)
                                         .FirstOrDefaultAsync(m => m.Id == id);

            if (job == null)
            {
                return NotFound();
            }
            Job = job;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Job).State = EntityState.Modified;

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
    }
}
