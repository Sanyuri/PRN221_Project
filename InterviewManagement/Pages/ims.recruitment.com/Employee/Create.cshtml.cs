using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using InterviewManagement.Models;
using InterviewManagement.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace InterviewManagement.Pages.ims.recruitment.com.user
{
    public class CreateModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;
        private readonly EmailService _emailService;

        public CreateModel(InterviewManagement.Models.InterviewManagementContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult OnGet()
        {
            if ( _context.Employee == null)
            {
                return NotFound();
            }
            ViewData["roleList"] = new SelectList(_context.Role.ToList(), "Id", "RoleName");
            ViewData["DepartmentList"] = new SelectList(_context.Department.ToList(), "Id", "DepertmentName");
            return Page();
        }

        [BindProperty]
        public EmployeeDto Employee { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Employee == null || Employee == null)
            {
                return Page();
            }
            var employeeToAdd = new Employee();
            employeeToAdd.FullName = Employee.FullName;
            employeeToAdd.Email = Employee.Email;
            employeeToAdd.dob = Employee.Dob;
            employeeToAdd.PhoneNumber = Employee.PhoneNumber;
            employeeToAdd.Address = Employee.Address;
            employeeToAdd.Gender = Employee.Gender;
            employeeToAdd.Role = _context.Role.Find(Employee.RoleId);
            employeeToAdd.Department = _context.Department.Find(Employee.DepartmentId);
            employeeToAdd.Status=Employee.Status;
            employeeToAdd.Note = Employee.Note;
            var lastEmployee = await _context.Employee
                                             .OrderByDescending(e => e.Id)
                                             .FirstOrDefaultAsync();
            employeeToAdd.UserName = GenerateUserName(Employee.FullName)+(lastEmployee?.Id+1);
            Debug.WriteLine(employeeToAdd.UserName);
            employeeToAdd.Password = "$10$9J9TnpcLedF0edPR1KYnl.6Lyq8lIovTXp7kJ7hwWyV2xdLhb54uW";
            try
            {
                _context.Employee.Add(employeeToAdd);
                await _context.SaveChangesAsync();
                await SendPasswordAsync(employeeToAdd.Email,employeeToAdd.UserName);

            }
            catch (DbUpdateConcurrencyException)
            {

                return NotFound();

            }
            return RedirectToPage("./Index");
        }

        private string GenerateUserName(string fullName)
        {
            string[] names = fullName.Split(' ');
            string lastname = names[names.Length-1];
            Debug.WriteLine(lastname);

            if (names.Length < 2) return fullName;
            string username = "";
            string middle = "";

            for (int i = 0; i < names.Length-1 ; i++)
            {
                middle += names[i][0].ToString().ToUpper();
            }
            username = lastname + middle;
            return username;
        }

        public async Task SendPasswordAsync(string email,string username)
        {
            string resetLink = Url.Page("/ResetPassword" , Request.Scheme);

            // Send the password reset email
            string subject = "no-reply-email-IMS-system <Account created>";
            string message = $"This email is from IMS system," +
                $"\r\nYour account has been created. Please use the following credential to \r\nlogin: \r\n" +
                $"• User name: {username}\r\n" +
                $"• Password: taokobiet123\r\n" +
                $"If anything wrong, please reach-out recruiter <offer recruiter owner \r\naccount>." +
                $" We are so sorry for this inconvenience.\r\nThanks & Regards!\r\nIMS Team.";

            await _emailService.SendEmailAsync(email, subject, message);
        }
    }
}
