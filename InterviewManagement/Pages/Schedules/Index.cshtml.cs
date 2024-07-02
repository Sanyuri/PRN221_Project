﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;
using InterviewManagement.Utils;
using Microsoft.Data.SqlClient;

namespace InterviewManagement.Pages.Schedules
{
    public class IndexModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;

        public IndexModel(InterviewManagement.Models.InterviewManagementContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string InterviewerFilter { get; set; }

        public PaginatedList<Schedule> Schedule { get; set; }
        public List<Employee> Employees { get; set; }

        public async Task OnGetAsync(string searchString, string status, string interviewer, int? pageNumber)
        {
            Keyword = searchString;
            StatusFilter = status;
            InterviewerFilter = interviewer;

            ViewData["Keyword"] = searchString;
            ViewData["StatusFilter"] = status;
            ViewData["InterviewerFilter"] = interviewer;

            IQueryable<Schedule> scheduleQuery = _context.Schedule
                .Include(j => j.Candidate)
                .Include(j => j.Job)
                .Include(j => j.Employees)
                .Where(j => j.IsDeleted == false);
            Employees = _context.Employee.ToList();

            if (!String.IsNullOrEmpty(Keyword))
            {
                scheduleQuery = scheduleQuery.Where(j => j.ScheduleName.Contains(Keyword));
            }

            if (!String.IsNullOrEmpty(StatusFilter))
            {
                scheduleQuery = scheduleQuery.Where(j => j.Status.Contains(StatusFilter));
            }

            if (!String.IsNullOrEmpty(InterviewerFilter))
            {
                scheduleQuery = scheduleQuery.Where(j => j.Employees.Any(e => e.FullName == InterviewerFilter));
            }

            int pageSize = 5;
            Schedule = await PaginatedList<Schedule>.CreateAsync(scheduleQuery.AsNoTracking(), pageNumber ?? 1, pageSize);
        }

        
    }
}