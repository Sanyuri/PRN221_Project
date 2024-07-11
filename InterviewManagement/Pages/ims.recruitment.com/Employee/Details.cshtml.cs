using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;
using InterviewManagement.Values;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace InterviewManagement.Pages.ims.recruitment.com.user
{
    [Authorize(Policy = "Admin")]

    public class DetailsModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;

        public DetailsModel(InterviewManagement.Models.InterviewManagementContext context)
        {
            _context = context;
        }

      public Employee Employee { get; set; } = default!;
        public IDictionary<int, string> status { get; } = StatusValue.UserStatus;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null || _context.Employee == null)
            {
                return NotFound();
            }
            ViewData["status"] = status;

            var employee = await _context.Employee
                .Include(c=>c.Department)
                .Include(c => c.Role)
                . FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }
            else 
            {
                Employee = employee;
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            long idChan = long.Parse(Request.Form["idChanfer"]);

            var employee = _context.Employee.Find(idChan);

            if (employee == null)
            {
                return NotFound();
            }
            String active = Request.Form["active"].ToString();
            employee.Status = active;
            try
            {
                _context.Attach(employee).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}
