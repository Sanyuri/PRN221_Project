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
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Diagnostics;

namespace InterviewManagement.Pages.candidate
{
    [Authorize(Policy = "Employee")]

    public class IndexModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;

        public IndexModel(InterviewManagement.Models.InterviewManagementContext context)
        {
            _context = context;
        }

        public IList<Candidate> Candidate { get; set; } = default!;
        public IDictionary<int, string> status { get; } = StatusValue.CandidateStatus;
        public string SearchTerm { get; set; } = string.Empty;
        public string StatusFilter { get; set; } = "All";

        // Pagination properties
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;

        public async Task OnGetAsync(int? pageNumber, string searchTerm, string statusFilter)
        {
            CurrentPage = pageNumber ?? 1;
            SearchTerm = searchTerm;
            StatusFilter = statusFilter;
            var sessionRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var user = await _context.Employee.Include(c => c.Role).Where(c => c.Role.RoleName == sessionRole).FirstOrDefaultAsync();

            var candidatesQuery = _context.Candidate
                .Include(c => c.Employee)
                .Include(c => c.Position)
                .Where(c => c.IsDeleted == false)
                .AsQueryable();

            if (user.Role?.RoleName == "Interviewer")
            {
                candidatesQuery = candidatesQuery.Where(c => c.Employee.Id == user.Id);
            }

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                candidatesQuery = candidatesQuery.Where(c =>
                    (c.FullName != null && c.FullName.Contains(SearchTerm)) ||
                    (c.Email != null && c.Email.Contains(SearchTerm)));
            }

            if (!string.IsNullOrEmpty(StatusFilter) && StatusFilter != "All")
            {
                candidatesQuery = candidatesQuery.Where(c => c.Status == StatusFilter);
            }

            var totalCandidates = await candidatesQuery.CountAsync();
            TotalPages = (int)Math.Ceiling(totalCandidates / (double)PageSize);

            Candidate = await candidatesQuery
                .OrderBy(c => Convert.ToInt32(c.Status))
                .ThenBy(c => c.CreatedOn)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
            if (user.Role?.RoleName == "Recruiter")
            {
                IDictionary<int, string> a = new Dictionary<int, string>()
                 {
            { 1, "Waiting for interview" },
            { 2, "Waiting for approval" },
            { 3, "Waiting for response" },
            { 4, "Open" },
            { 8, "Accepted offer" },
            { 9, "Declined offer" },
            { 10, "Cancelled offer" },
            { 13, "Banned" }
                 };
                ViewData["statuss"] = a;
            }
            else if (user.Role?.RoleName == "Manager")
            {
                IDictionary<int, string> a = new Dictionary<int, string>()
                 {
            { 8, "Accepted offer" },
            { 9, "Declined offer" },
                 };
                ViewData["statuss"] = a;
            }else if(user.Role?.RoleName == "Interviewer")
            {
                IDictionary<int, string> a = new Dictionary<int, string>()
                 {
                       { 5, "Passed" },
                       { 11, "Failed interview" },
                       { 12, "Cancelled interview" },

                 };
                ViewData["statuss"] = a;
            }
            else
            {
                ViewData["statuss"] = status;

            }
            ViewData["status"] = status;

            ViewData["User"] = user?.Role?.RoleName;
        }

        public IActionResult OnPost()
        {
            string searchTerm = Request.Form["searchTerm"];
            string statusFilter = Request.Form["filter"];

            return RedirectToPage(new { pageNumber = 1, searchTerm, statusFilter });
        }
    }
}
