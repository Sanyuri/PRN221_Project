using InterviewManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace InterviewManagement.Pages.ims.recruitment.com
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly InterviewManagementContext _context;
        private readonly EmailService _emailService;

        public ForgotPasswordModel(InterviewManagementContext context, EmailService emailService)
        {
            this._context = context;
            this._emailService = emailService;

        }

        [BindProperty]
        public ForgotPassword forgotPassword { get; set; } = default!;

        public class ForgotPassword
        {
            [Required(ErrorMessage ="Email is required")]
            [EmailAddress]
            public string? Email { get; set; }
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Email is not valid");
                return Page();
            }

            Employee? employee = await _context.Employee.Where(e=> e.Email.Equals(forgotPassword.Email)).FirstOrDefaultAsync();
            if(employee == null)
            {
                ModelState.AddModelError(string.Empty, "This Email is not existed");
                return Page();
            }
            else
            {
                string token = Guid.NewGuid().ToString();
                string url = "https://localhost:7186/ims.recruitment.com/resetpassword?token="+token;
                employee.ResetPasswordToken = token;
                employee.ExpiredDate = DateTime.Now.AddDays(1);
                //Update employee
                _context.Attach(employee).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                await SendForgotPasswordLinkAsync(employee.Email,url, employee.FullName);
                ModelState.AddModelError("success", "We've sent an email with the link to reset your password.");
                return Page();
            }
        }

        public async Task SendForgotPasswordLinkAsync(string? email, string? url, string? fullName)
        {
            string subject = "no-reply-email-IMS-system <Account created>";
            String mailContent = "<p>We have just received a password reset request for " + fullName + ".</p>"
            + "<p>Please click "
                + "<a href='" + url + "'>here</a>"
                + " to reset your password</p>"
                + "<p>For your security, the link will expire in 24 hours or immediately after you reset your password.</p>"
                + "<p>Thanks & Regards!</p>"
                + "<p>IMS Team.”<</p>";

            await _emailService.SendEmailAsync(email, subject, mailContent);
        }
    }
}
