using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using InterviewManagement.Models;

namespace InterviewManagement.Pages.ims.recruitment.com.test
{
    public class CreateModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;

        public CreateModel(InterviewManagement.Models.InterviewManagementContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["CandidateId"] = new SelectList(_context.Candidate, "Id", "Address");
        ViewData["ContractId"] = new SelectList(_context.Contract, "Id", "ContractName");
        ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "DepertmentName");
        ViewData["LevelId"] = new SelectList(_context.Level, "Id", "LevelName");
        ViewData["PositionId"] = new SelectList(_context.Position, "Id", "PositionName");
        ViewData["ScheduleId"] = new SelectList(_context.Schedule, "Id", "Location");
            return Page();
        }

        [BindProperty]
        public Offer Offer { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Offer == null || Offer == null)
            {
                return Page();
            }

            _context.Offer.Add(Offer);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
