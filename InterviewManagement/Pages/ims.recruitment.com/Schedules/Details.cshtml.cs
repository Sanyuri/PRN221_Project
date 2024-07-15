using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;
using Microsoft.AspNetCore.Authorization;

namespace InterviewManagement.Pages.Schedules
{
    [Authorize(Policy = "Employee")]
    public class DetailsModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;
        private readonly EmailService _emailService;

        public DetailsModel(InterviewManagement.Models.InterviewManagementContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [BindProperty]
        public Schedule Schedule { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null || _context.Schedule == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule
                .Include(j => j.Candidate)
                .Include(j => j.Job)
                .Include(j => j.Employees).FirstOrDefaultAsync(m => m.Id == id);

            if (schedule == null)
            {
                return NotFound();
            }
            else
            {
                Schedule = schedule;
            }
            return Page();
        }

        public IActionResult OnPostSubmitResult()
        {
            var schedule = _context.Schedule
                       .Include(s => s.Candidate)
                       .FirstOrDefault(s => s.Id == Schedule.Id);

            if (schedule != null)
            {
                schedule.Note = Schedule.Note;
                schedule.Result = Schedule.Result;

                if (schedule.Result == "Passed")
                {
                    schedule.Candidate.Status = "5";
                }
                else if (schedule.Result == "Failed")
                {
                    schedule.Candidate.Status = "11";
                }

                schedule.Status = "Closed";
                _context.SaveChanges();
            }

            TempData["SuccessMessage"] = "Schedule result have been submited ";

            return RedirectToPage("./Details", new { Schedule.Id });
        }

        private bool ScheduleExists(long id)
        {
            return _context.Schedule.Any(e => e.Id == id);
        }

        public async Task<IActionResult> OnPostSendReminderAsync(long id)
        {
            var schedule = await _context.Schedule
                .Include(s => s.Candidate)
                .Include(s => s.Employees)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (schedule == null)
            {
                return NotFound();
            }

            var title = schedule.ScheduleName;
            var date = schedule.ScheduleTime;
            var url = "https://localhost:7186/ims.recruitment.com/Schedules/Details?id=" + schedule.Id;
            foreach (var employee in schedule.Employees)
            {
                var email = employee.Email;
                await SendReminderAsync(email, title, date, url, schedule.Id);              
            }

            schedule.Status = "Invited";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Mail sent successfully!";
            return RedirectToPage("./Details", new { id = schedule.Id });
        }

        private async Task SendReminderAsync(string? email, string? title, DateTime? date, string url, long scheduleId)
        {
            string subject = "no-reply-email-IMS-system <Reminder Schedule>";
            string message = "<p>This email is from IMS system</p>" +
                "<p>Remind for the incoming interview schedule of you: </p>" +
                "<p>• Title: " + title + "</p>" +
                "<p>• Date: " + date + "</p>" +
                "<p>Click " +
                "<a href='" + url +"'>here</a> " +
                " to see details</p>" +
                "<p>If anything wrong, please reach out to the recruiter. We are so sorry for this inconvenience.</p>" +
                "<p>Thanks & Regards!</p>" +
                "<p>IMS Team.</p>";

            await _emailService.SendEmailAsync(email, subject, message);
        }
    }
}
