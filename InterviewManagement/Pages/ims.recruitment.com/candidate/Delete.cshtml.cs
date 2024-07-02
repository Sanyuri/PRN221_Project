using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;
using System.Diagnostics;

namespace InterviewManagement.Pages.candidate
{
    public class DeleteModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;

        public DeleteModel(InterviewManagement.Models.InterviewManagementContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Candidate Candidate { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Candidate = await _context.Candidate.FirstOrDefaultAsync(m => m.Id == id);

            if (Candidate == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Debug.WriteLine(Request.Form["idChanfer"]);
            long id = long.Parse(Request.Form["idChanfer"]);

            var candidate = await _context.Candidate.FindAsync(id);

            if (candidate == null)
            {
                return NotFound();
            }

            candidate.IsDeleted = true;

            try
            {
                _context.Attach(candidate).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}
