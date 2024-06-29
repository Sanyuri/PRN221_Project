using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;
using InterviewManagement.Dtos;

namespace InterviewManagement.Pages.ims.recruitment.com.Offers
{
    public class IndexModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;

        public IndexModel(InterviewManagement.Models.InterviewManagementContext context)
        {
            _context = context;
        }

        public IList<OfferDto> Offer { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Offer != null)
            {
                Offer = await _context.Offer
                               .Include(o => o.Candidate)
                               .Include(o => o.Department)
                               .Include(o => o.Employees)
                               .Select(o => new OfferDto
                               {
                                   Id = o.Id,
                                   CandidateName = o.Candidate.FullName,
                                   Email = o.Candidate.Email,
                                   Approver = o.Employees.FirstOrDefault(e => e.Role.RoleName.Equals("Manager")).FullName,
                                   Department = o.Department.DepertmentName,
                                   Note = o.Note,
                                   Status = o.Status
                               })
                               .ToListAsync();
            }
        }
    }
}
