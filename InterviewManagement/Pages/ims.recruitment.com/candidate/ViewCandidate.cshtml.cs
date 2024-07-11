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
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace InterviewManagement.Pages.ims.recruitment.com.candidate
{
    [Authorize(Policy = "Employee")]

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
            var user = await _context.Employee.Include(c => c.Role).FirstOrDefaultAsync(c => c.Id == 2);
            ViewData["User"] = user.Role.RoleName;


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
            string ModifiedBy = Request.Form["ModifiedBy"].ToString();
            DateTime CreatedOn = DateTime.Parse(Request.Form["CreatedOn"]);

            var candidateToUpdate = _context.Candidate.Find(idChan);
            if (candidateToUpdate == null)
            {
                return NotFound();
            }
            candidateToUpdate.Status = 13.ToString();
            candidateToUpdate.ModifiedBy=ModifiedBy;
            candidateToUpdate.CreatedOn= CreatedOn;
            Debug.WriteLine(candidateToUpdate.ModifiedBy);
            
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
