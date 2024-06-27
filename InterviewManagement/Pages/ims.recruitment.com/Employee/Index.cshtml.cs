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
        private readonly InterviewManagement.Models.InterviewManagementContext _context;

        public IndexModel(InterviewManagement.Models.InterviewManagementContext context)
        {
            _context = context;
        }

        public IList<Employee> Employee { get;set; } = default!;
        public string SearchTerm { get; set; } = string.Empty;
        public string StatusFilter { get; set; } = "All";
        public IDictionary<int, string> status { get; } = StatusValue.UserStatus;

        public async Task OnGetAsync()
        {
            ViewData["roleList"] = new SelectList(await _context.Role.ToListAsync(), "Id", "RoleName");
            ViewData["status"] = status;

            if (_context.Employee != null)
            {
                Employee = await _context.Employee.ToListAsync();
            }
        }
        public void OnPost()
        {
            string searchTerm = Request.Form["searchTerm"];
            string filter = Request.Form["filter"];
            SearchTerm = searchTerm;
            StatusFilter = filter;
            LoadData(SearchTerm, StatusFilter);
            ViewData["roleList"] = new SelectList( _context.Role.ToList(), "Id", "RoleName");
            ViewData["status"] = status;
            ViewData["searchTerm"] = SearchTerm;
            ViewData["statusFilter"] = StatusFilter;
        }
        private void LoadData(string searchTerm, string statusFilter)
        {
            var employeeQuery = _context.Employee              
                .Include(c => c.Role)
                .AsQueryable();

            //AsQueryable tao 1 doi tuong truy van va co the soan thao va sua doi trc khi thuc thi
            // Apply search filter
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                employeeQuery = employeeQuery.Where(c =>
            (c.FullName != null && c.FullName.Contains(SearchTerm)) ||
            (c.Email != null && c.Email.Contains(SearchTerm)));
            }
            if (!string.IsNullOrEmpty(StatusFilter) && StatusFilter != "All")
            {
                employeeQuery = employeeQuery.Where(c => c.Role.Id.ToString() == StatusFilter);
            }
            Employee = employeeQuery.ToList();
        }
    }
}
