using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;

namespace InterviewManagement.Pages.ims.recruitment.com.Test
{
    public class EditModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;

        public EditModel(InterviewManagement.Models.InterviewManagementContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Offer Offer { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Offer == null)
            {
                return NotFound();
            }

            var offer =  await _context.Offer.FirstOrDefaultAsync(m => m.Id == id);
            if (offer == null)
            {
                return NotFound();
            }
            Offer = offer;
           ViewData["CandidateId"] = new SelectList(_context.Candidate, "Id", "Address");
           ViewData["ContractId"] = new SelectList(_context.Contract, "Id", "ContractName");
           ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "DepertmentName");
           ViewData["LevelId"] = new SelectList(_context.Level, "Id", "LevelName");
           ViewData["PositionId"] = new SelectList(_context.Position, "Id", "PositionName");
           ViewData["ScheduleId"] = new SelectList(_context.Schedule, "Id", "Location");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Offer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OfferExists(Offer.Id))
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

        private bool OfferExists(int id)
        {
          return (_context.Offer?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
