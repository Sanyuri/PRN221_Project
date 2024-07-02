using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using InterviewManagement.Values;

namespace InterviewManagement.Pages.ims.recruitment.com.user
{
    public class IndexModel : PageModel
    {
        private readonly InterviewManagementContext _context;

        public IndexModel(InterviewManagementContext context)
        {
            _context = context;
        }

        public IList<Employee> Employee { get; set; } = default!;
        public string SearchTerm { get; set; } = string.Empty;
        public string StatusFilter { get; set; } = "All";
        public IDictionary<int, string> Status { get; } = StatusValue.UserStatus;

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 5;

        public async Task OnGetAsync(int? pageNumber, string searchTerm, string statusFilter)
        {
            SearchTerm = searchTerm;
            StatusFilter = statusFilter ?? "All";
            CurrentPage = pageNumber ?? 1;

            await LoadDataAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            SearchTerm = Request.Form["searchTerm"];
            StatusFilter = Request.Form["filter"];
            CurrentPage = 1;

            await LoadDataAsync();

            return Page();
        }

        private async Task LoadDataAsync()
        {
            var employeeQuery = _context.Employee
                .Include(e => e.Role)
                .AsQueryable();

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                employeeQuery = employeeQuery.Where(e =>
                    (e.FullName != null && e.FullName.Contains(SearchTerm)) ||
                    (e.Email != null && e.Email.Contains(SearchTerm)));
            }

            if (!string.IsNullOrEmpty(StatusFilter) && StatusFilter != "All")
            {
                employeeQuery = employeeQuery.Where(e => e.Role.Id.ToString() == StatusFilter);
            }

            var totalEmployee = await employeeQuery.CountAsync();
            TotalPages = (int)Math.Ceiling(totalEmployee / (double)PageSize);

            Employee = await employeeQuery
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewData["roleList"] = new SelectList(await _context.Role.ToListAsync(), "Id", "RoleName");
            ViewData["status"] = Status;
            ViewData["searchTerm"] = SearchTerm;
            ViewData["statusFilter"] = StatusFilter;
        }
    }
}
