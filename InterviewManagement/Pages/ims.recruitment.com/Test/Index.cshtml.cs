using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;

namespace InterviewManagement.Pages.ims.recruitment.com.Test
{
    public class IndexModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;

        public IndexModel(InterviewManagement.Models.InterviewManagementContext context)
        {
            _context = context;
        }

        public IList<Offer> Offer { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Offer != null)
            {
                Offer = await _context.Offer
                .Include(o => o.Candidate)
                .Include(o => o.Contract)
                .Include(o => o.Department)
                .Include(o => o.Level)
                .Include(o => o.Position)
                .Include(o => o.Schedule).ToListAsync();
            }
        }
    }
}
