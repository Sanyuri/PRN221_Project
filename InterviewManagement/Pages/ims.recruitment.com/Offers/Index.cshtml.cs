using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;
using InterviewManagement.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging;
using System.ComponentModel.DataAnnotations;
using InterviewManagement.Values;

namespace InterviewManagement.Pages.ims.recruitment.com.Offers
{
    public class IndexModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;

        public IndexModel(InterviewManagement.Models.InterviewManagementContext context)
        {
            _context = context;
        }
        [BindProperty]
        public long ApproverId { get; set; } = default!;

        [BindProperty]
        public long RecruiterId { get; set; } = default!;


        public IList<Offer> Offers { get;set; } = default!;

        [BindProperty]
        public Offer Offer { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Offer != null)
            {
                ViewData["Candidate"] = new SelectList(_context.Candidate,"Id","FullName");
                ViewData["Contract"] = new SelectList(_context.Contract, "Id", "ContractName");
                ViewData["Position"] = new SelectList(_context.Position, "Id", "PositionName");
                ViewData["Level"] = new SelectList(_context.Level, "Id", "LevelName");
                ViewData["InterviewInfo"] = new SelectList(_context.Schedule, "Id", "ScheduleName");
                ViewData["Department"] = new SelectList(_context.Department, "Id", "DepertmentName");
                ViewData["Approver"] = new SelectList(_context.Employee.Where(r => r.Role.RoleName.Equals("Manager")), "Id", "FullName");
                ViewData["Recruiter"] = new SelectList(_context.Employee.Where(r => r.Role.RoleName.Equals("Recruiter")), "Id", "FullName");
                ViewData["ScheduleNote"] = _context.Schedule.ToDictionary(s => s.Id, s => s.Note);
                Offers = await  _context.Offer
                               .Include(o => o.Candidate)
                               .Include(o => o.Department)
                               .Include(o => o.Employees).ThenInclude(r => r.Role)
                               .Include(o => o.Level)
                               .Include(o => o.Schedule)
                               .Include(o => o.Contract)
                               .ToListAsync();
                
            }
        }
        public IActionResult OnPostAddOffer()
        {
            if(!ModelState.IsValid || _context.Offer == null || Offer == null)
            {
                return RedirectToPage("./Index");
            }
            Employee Approver = _context.Employee.Find(ApproverId);
            Employee Recruiter = _context.Employee.Find(RecruiterId);
                Offer.Employees.Add(Approver);
                Offer.Employees.Add(Recruiter);
            Offer.Status = "Waiting for approval";
            Offer.IsDeleted = false;
                _context.Add(Offer);
            //change Candidate's status
            Candidate candidate = _context.Candidate.Find(Offer.CandidateId);
            candidate.Status = "Waiting for approval";
            _context.Update(candidate);
                 _context.SaveChanges();
            return RedirectToPage("./Index");
        }

        public IActionResult OnPostUpdateOffer()
        {
            if (!ModelState.IsValid || _context.Offer == null || Offer == null)
            {
                return RedirectToPage("./Index");
            }
            Offer.Employees = new List<Employee>();
            Employee Approver = _context.Employee.Find(ApproverId);
            Employee Recruiter = _context.Employee.Find(RecruiterId);
            Offer.Employees.Add(Approver);
            Offer.Employees.Add(Recruiter);
            _context.Attach(Offer).State = EntityState.Modified;
            //change Candidate's status
            _context.SaveChanges();
            return RedirectToPage("./Index");
        }

        public IActionResult OnPostUpdateOfferStatus()
        {
            if (!ModelState.IsValid || _context.Offer == null || Offer == null)
            {
                return RedirectToPage("./Index");
            }
            //change Candidate's status
            if (Offer.Status.Equals("Waiting for Response"))
            {
                Candidate candidate = _context.Candidate.Find(Offer.CandidateId);
                candidate.Status = "Waiting for Response";
                _context.Update(candidate);
            }
            if (Offer.Status.Equals("Accepted"))
            {
                Candidate candidate = _context.Candidate.Find(Offer.CandidateId);
                candidate.Status = "Accepted Offer";
                _context.Update(candidate);
            }
            if (Offer.Status.Equals("Declined"))
            {
                Candidate candidate = _context.Candidate.Find(Offer.CandidateId);
                candidate.Status = "Declined Offer";
                _context.Update(candidate);
            }
            _context.Attach(Offer).State = EntityState.Modified;          
            _context.SaveChanges();
            return RedirectToPage("./Index");
        }
    }
}
