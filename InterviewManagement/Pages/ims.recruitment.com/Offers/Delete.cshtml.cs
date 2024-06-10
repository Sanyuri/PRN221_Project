using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;

namespace InterviewManagement.Pages.ims.recruitment.com.Offers
{
    public class DeleteModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;

        public DeleteModel(InterviewManagement.Models.InterviewManagementContext context)
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

            var offer = await _context.Offer.FirstOrDefaultAsync(m => m.Id == id);

            if (offer == null)
            {
                return NotFound();
            }
            else 
            {
                Offer = offer;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Offer == null)
            {
                return NotFound();
            }
            var offer = await _context.Offer.FindAsync(id);

            if (offer != null)
            {
                Offer = offer;
                _context.Offer.Remove(Offer);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
