using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;
using InterviewManagement.Values;
using InterviewManagement.DTOs;

namespace InterviewManagement.Pages.candidate
{
    public class ViewCandidateModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;

        public ViewCandidateModel(InterviewManagement.Models.InterviewManagementContext context)
        {
            _context = context;
        }

      public Candidate Candidate { get; set; } = default!;
        public IDictionary<int, string> status { get; } = StatusValue.CandidateStatus;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null || _context.Candidate == null)
            {
                return NotFound();
            }

            var candidate = await _context.Candidate
                .Include(c => c.Position)
                .Include(c=>c.Skills)
                 .Include(c => c.Employee) 
                 .Include(c => c.HighestLevel)
                 .FirstOrDefaultAsync(m => m.Id == id);
            if (candidate == null)
            {
                return NotFound();
            }
            else 
            {
                Candidate = candidate;
            }
            ViewData["status"] = status;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            long idChan = long.Parse(Request.Form["idChanfer"]);

            var candidateToUpdate = _context.Candidate.Find(idChan);

            if (candidateToUpdate == null)
            {
                return NotFound();
            }
            candidateToUpdate.Status = 13.ToString();
            try
            {
                _context.Attach(candidateToUpdate).State = EntityState.Modified;
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
