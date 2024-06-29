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
using Google.Apis.Drive.v3.Data;

namespace InterviewManagement.Pages.candidate
{
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
        public async Task OnGetAsync(int? pageNumber)
        {
            CurrentPage = pageNumber ?? 1;
            var user = await _context.Employee.Include(c => c.Role).FirstOrDefaultAsync(c => c.Id == 2);

            var candidatesQuery = _context.Candidate
             .Include(c => c.Employee)
             .Include(c => c.Position)
             .Where(c=>c.IsDeleted==false)
             .AsQueryable();

            if (user.Role?.RoleName == "Interviewer")
            {
                candidatesQuery = candidatesQuery.Where(c => c.Employee.Id == user.Id);
            }
            var totalCandidates = await candidatesQuery.CountAsync();
            TotalPages = (int)Math.Ceiling(totalCandidates / (double)PageSize);
            Candidate = await candidatesQuery
                  .OrderBy(c => Convert.ToInt32(c.Status))
                  .ThenBy(c => c.CreatedOn)
                  .Skip((CurrentPage - 1) * PageSize)
                  .Take(PageSize)
                  .ToListAsync();
            ViewData["status"] = status;
            ViewData["User"] = ViewData["User"] = user?.Role?.RoleName;
        }
        public void OnPost()
        {
            string searchTerm = Request.Form["searchTerm"];
            string filter = Request.Form["filter"];
            SearchTerm = searchTerm;
            StatusFilter = filter;
            LoadData(SearchTerm, StatusFilter);
            ViewData["status"] = status;
            ViewData["searchTerm"] = SearchTerm;
            ViewData["statusFilter"] = StatusFilter;
        }
        private void LoadData(string searchTerm, string statusFilter)
        {
            var user = _context.Employee.Include(c => c.Role).FirstOrDefault(c => c.Id == 2);
            var candidatesQuery = _context.Candidate
                .Include(c => c.Employee)
                .Include(c => c.Position)
                .OrderBy(c => Convert.ToInt32(c.Status))
                .ThenBy(c => c.CreatedOn)
                .AsQueryable();

            if (user?.Role?.RoleName == "Interviewer")
            {
                candidatesQuery = candidatesQuery.Where(c => c.Employee.Id == user.Id);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                candidatesQuery = candidatesQuery.Where(c =>
                    (c.FullName != null && c.FullName.Contains(searchTerm)) ||
                    (c.Email != null && c.Email.Contains(searchTerm)));
            }

            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                candidatesQuery = candidatesQuery.Where(c => c.Status == statusFilter);
                var totalCandidates = candidatesQuery.Count();
                TotalPages = (int)Math.Ceiling(totalCandidates / (double)PageSize);

            }


            Candidate = candidatesQuery.ToList();
        }

    }
}
