using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;

namespace InterviewManagement.Pages.Schedules
{
    public class DetailsModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;
        private readonly EmailService _emailService;

        public DetailsModel(InterviewManagement.Models.InterviewManagementContext context)
        {
            _context = context;
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var scheduleToUpdate = await _context.Schedule.FirstOrDefaultAsync(s => s.Id == Schedule.Id);

            if (scheduleToUpdate == null)
            {
                return NotFound();
            }
          
            scheduleToUpdate.Note = Schedule.Note;
            scheduleToUpdate.Result = Schedule.Result;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScheduleExists(Schedule.Id))
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

        private bool ScheduleExists(long id)
        {
            return (_context.Schedule?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> OnPostSendReminderAsync(long id)
        {
            var schedule = await _context.Schedule
                .Include(j => j.Candidate)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (schedule == null)
            {
                return NotFound();
            }
           
            var title = schedule.ScheduleName;
            foreach(var employee in Schedule.Employees)
            {
                var email = employee.Email;
                await SendReminderAsync(email, title, id);
            }
         
            return RedirectToPage("./Details", new { id });
        }

        public async Task SendReminderAsync(string email, string title, long scheduleId)
        {
            string resetLink = Url.Page("/Schedules/Details", null, new { id = scheduleId }, Request.Scheme);

            // Send the password reset email
            string subject = "no-reply-email-IMS-system <Reminder Schedule>";
            string message = $"This email is from IMS system," +
                $"\r\nRemind for the incoming interview schedule of you: \r\n" +
                $"• Title: {title}\r\n" +
                $"If anything wrong, please reach-out recruiter <offer recruiter owner \r\naccount>." +
                $" We are so sorry for this inconvenience.\r\nThanks & Regards!\r\nIMS Team.";

            await _emailService.SendEmailAsync(email, subject, message);
        }
    }
}
