using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;
using System.Diagnostics;
using InterviewManagement.Dtos;
using InterviewManagement.DTOs;

namespace InterviewManagement.Pages.ims.recruitment.com.user
{
    public class EditModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;

        public EditModel(InterviewManagement.Models.InterviewManagementContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EmployeeDto Employee { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null || _context.Employee == null)
            {
                return NotFound();
            }

            var employee =  await _context.Employee
                .Include(x => x.Role)
                .Include(x => x.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["roleList"] = new SelectList(_context.Role.ToList(), "Id", "RoleName");
            ViewData["DepartmentList"] = new SelectList(_context.Department.ToList(), "Id", "DepertmentName");
            Employee = EmployeeDto.FromCandidate(employee);
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    Debug.WriteLine($"Field: {entry.Key}, Error: {string.Join(", ", entry.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                ViewData["roleList"] = new SelectList(_context.Role.ToList(), "Id", "RoleName");
                ViewData["DepartmentList"] = new SelectList(_context.Department.ToList(), "Id", "DepertmentName");
                return Page();
            }
           
            long idChan = long.Parse(Request.Form["idChanfer"]);
            var employeeToUpdate = _context.Employee.Find(idChan);
            if (employeeToUpdate == null)
            {
                return NotFound();
            }
            employeeToUpdate.FullName=Employee.FullName;
            employeeToUpdate.Email = Employee.Email;
            employeeToUpdate.dob = Employee.Dob;
            employeeToUpdate.PhoneNumber = Employee.PhoneNumber;
            employeeToUpdate.Address = Employee.Address;
            employeeToUpdate.Role = _context.Role.Find(Employee.RoleId);
            employeeToUpdate.Department = _context.Department.Find(Employee.DepartmentId);

            try
            {           
                _context.Attach(employeeToUpdate).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(Employee.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool EmployeeExists(long id)
        {
          return (_context.Employee?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
