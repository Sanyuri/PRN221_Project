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
using Microsoft.AspNetCore.Authorization;

namespace InterviewManagement.Pages.ims.recruitment.com.user
{
    [Authorize(Policy = "Admin")]

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
            ViewData["roleList"] = new SelectList(_context.Role.Where(c=>c.RoleName!="Candidate").ToList(), "Id", "RoleName");
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
            var s = _context.Employee.Where(e => e.FullName == employeeToAdd.FullName);
            employeeToAdd.UserName = GenerateUserName(Employee.FullName)+(s.Count()+1);
            Debug.WriteLine(employeeToAdd.UserName);
            string randomPassword = GenerateRandomPassword(7);
            employeeToAdd.Password = BCrypt.Net.BCrypt.HashPassword(randomPassword);

            try
            {
                _context.Employee.Add(employeeToAdd);
                await _context.SaveChangesAsync();
                await SendPasswordAsync(employeeToAdd.Email,employeeToAdd.UserName,randomPassword);

            }
            catch (DbUpdateConcurrencyException)
            {

                return NotFound();

            }
            return RedirectToPage("./Index");
        }
        private string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
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

        public async Task SendPasswordAsync(string email,string username, string password)
        {

            // Send the password reset email
            string subject = "no-reply-email-IMS-system <Account created>";
            string message = $"This email is from IMS system, <br/>" +
                $"\r\nYour account has been created. Please use the following credential to login:<br/> " +
                $"• User name: {username}\r\n  <br/>" +
                $"• Password: {password}\r\n  <br/>" +
                $"If anything wrong, please reach-out recruiter <offer recruiter owner \r\naccount>.  <br/>" +
                $" We are so sorry for this inconvenience.\r\nThanks & Regards!\r\nIMS Team.  <br/>";

            await _emailService.SendEmailAsync(email, subject, message);
        }
    }
}
