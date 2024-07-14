using InterviewManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace InterviewManagement.Pages.ims.recruitment.com
{
    public class ResetPasswordModel : PageModel
    {
        private InterviewManagementContext _context;

        public ResetPasswordModel(InterviewManagementContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ResetPassword ResetPasword { get; set; } = default!;

        public class ResetPassword
        {
            public string? Token { get; set; }
            [Required(ErrorMessage ="Password is required")]
            public string? Password { get; set; }

            [Required(ErrorMessage ="Re-password is required")]
            public string? RePassword { get; set; }
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var AccountId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (AccountId == null)
            {
                //Reset password for forgetpassword function
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Employee? employee = await _context.Employee.Where(e => e.ResetPasswordToken.Equals(ResetPasword.Token)).FirstOrDefaultAsync();
                if (employee == null)
                {
                    ModelState.AddModelError(string.Empty, "Cannot update user's password");
                    return Page();
                }
                if (!ResetPasword.Password.Equals(ResetPasword.RePassword))
                {
                    ModelState.AddModelError(string.Empty, "Password and Confirm password don’t match. Please try again.");
                    return Page();
                }
                if (employee.ExpiredDate < DateTime.Now)
                {
                    ModelState.AddModelError(string.Empty, "This link has expired. Please go back to Homepage and try again.");
                    return Page();
                }              
                employee.ResetPasswordToken = null;
                employee.ExpiredDate = null;
                employee.Password = BCrypt.Net.BCrypt.HashPassword(ResetPasword.Password);
                _context.Attach(employee).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToPage("/Index");
            }
            else
            {
                //Reset password for user's reset password function
                Employee? employee = await _context.Employee.Where(e => e.Id == long.Parse(AccountId)).FirstOrDefaultAsync();
                if(employee == null)
                {
                    return NotFound();
                }
                employee.Password = BCrypt.Net.BCrypt.HashPassword(ResetPasword.Password);
                _context.Attach(employee).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToPage("/ims.recruitment.com/HomePage");
            }           
        }
    }
}
