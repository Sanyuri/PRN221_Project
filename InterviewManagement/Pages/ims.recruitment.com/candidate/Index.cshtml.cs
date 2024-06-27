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
        public async Task OnGetAsync()
        {
            if (_context.Candidate != null)
            {
                Candidate = await _context.Candidate
                    .Include(c => c.Employee)
                    .Include(c => c.Position).ToListAsync();
            }
            ViewData["status"] = status;

        }
        public void OnPost()
        {
            string searchTerm = Request.Form["searchTerm"];
            string filter = Request.Form["filter"];
            SearchTerm = searchTerm;
            StatusFilter =filter;
            LoadData(SearchTerm, StatusFilter);
            ViewData["status"] = status;
            ViewData["searchTerm"] = SearchTerm;
            ViewData["statusFilter"] = StatusFilter;
        }
        private void LoadData(string searchTerm,string statusFilter)
        {
            var candidatesQuery = _context.Candidate
                .Include(c => c.Employee)
                .Include(c => c.Position)
                .AsQueryable();

            //AsQueryable tao 1 doi tuong truy van va co the soan thao va sua doi trc khi thuc thi
            // Apply search filter
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
            Candidate = candidatesQuery.ToList();
        }
        
    }
}
