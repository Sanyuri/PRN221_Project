using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;

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
            if (id == null || _context.Candidate == null)
            {
                return NotFound();
            }
            var candidate = _context.Candidate.Find(id);

            if (candidate == null)
            {
                return NotFound();
            }
            else 
            {
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
            }
            return RedirectToPage("./Index");
        }

       
    }
}
