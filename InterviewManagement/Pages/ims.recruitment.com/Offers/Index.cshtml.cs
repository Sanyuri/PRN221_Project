using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;
using InterviewManagement.Values;
using InterviewManagement.Dtos;
using InterviewManagement.Services;

namespace InterviewManagement.Pages.ims.recruitment.com.Offers
{
    public class IndexModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;
        private OfferService offerService = new OfferServiceImpl();

        public IndexModel(InterviewManagement.Models.InterviewManagementContext context)
        {
            _context = context;
        }

        public ICollection<Offer> Offer { get;set; } = default!;
        public ICollection<string> CandidateStatus = StatusValue.CanidateStatus;
        public async Task OnGetAsync()
        {
            if (_context.Offer != null)
            {
                Offer = await _context.Offer.Include(c => c.Candidate).ToListAsync();
            }
        }
    }
}
