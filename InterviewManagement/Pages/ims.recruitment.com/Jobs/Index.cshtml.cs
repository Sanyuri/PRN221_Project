using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;
using InterviewManagement.Utils;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
using System.Security.Claims;

namespace InterviewManagement.Pages.Jobs
{
    [Authorize(Policy = "Job")]
    public class IndexModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;

        public IndexModel(InterviewManagement.Models.InterviewManagementContext context)
        {
            _context = context;
        }
        [BindProperty]
        public IFormFile ExcelFile { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; }

        public PaginatedList<Job> Job { get; set; }

        public async Task OnGetAsync(string sortOrder, string searchString, string status, int? pageNumber)
        {
            SortOrder = sortOrder;
            Keyword = searchString;
            StatusFilter = status;

            ViewData["CurrentSort"] = sortOrder;
            ViewData["Keyword"] = searchString;
            ViewData["StatusFilter"] = status;

            await UpdateJobStatusesAsync();

            IQueryable<Job> jobQuery = _context.Job
                .Include(j => j.Skills)
                .Include(j => j.Levels)
                .Where(j => j.IsDeleted == false);

            // Search
            if (!String.IsNullOrEmpty(Keyword))
            {
                jobQuery = jobQuery.Where(j => j.JobName.Contains(Keyword));
            }

            if (!String.IsNullOrEmpty(StatusFilter))
            {
                jobQuery = jobQuery.Where(j => j.Status.Contains(StatusFilter));
            }

            // Sorting
            switch (SortOrder)
            {
                default:
                    jobQuery = jobQuery.OrderBy(s => s.Status == "Open" ? 1 : s.Status == "Draft" ? 2 : 3)
                                       .ThenByDescending(s => s.StartDate);
                    break;
            }

            int pageSize = 10;
            Job = await PaginatedList<Job>.CreateAsync(jobQuery.AsNoTracking(), pageNumber ?? 1, pageSize);
        }

        private async Task UpdateJobStatusesAsync()
        {
            var jobs = await _context.Job.ToListAsync();
            var currentDate = DateTime.UtcNow;

            foreach (var job in jobs)
            {
                if (job.StartDate <= currentDate && job.EndDate >= currentDate)
                {
                    job.Status = "Open";
                }
                else if (currentDate > job.EndDate)
                {
                    job.Status = "Close";
                }
            }

            await _context.SaveChangesAsync();
        }

        public IActionResult OnPostDelete(int jobId)
        {
            var job = _context.Job.Find(jobId);

            if (job != null)
            {
                job.IsDeleted = true;
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Job deleted successfully!";
            }
            
            return RedirectToPage("Index");
        }
        
        public async Task<IActionResult> OnPostImportJob()
        {
            if (ExcelFile == null || ExcelFile.Length == 0)
            {
                TempData["MessageType"] = "danger";
                TempData["Message"] = "File is not existed";
                return RedirectToPage();
            }
            List<Job> jobs = new List<Job>();
            var AccountId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            Employee? employee = _context.Employee.Find(long.Parse(AccountId));
            try
            {
                using (var stream = new MemoryStream())
                {
                    await ExcelFile.CopyToAsync(stream);
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        int rowCount = worksheet.Dimension.Rows;
                        int colCount = worksheet.Dimension.Columns;
                        List<Skill> skills = await _context.Skill.ToListAsync();
                        List<Benefit> benefits = await _context.Benefit.ToListAsync();
                        List<Level> levels = await _context.Level.ToListAsync();

                        for (int row = 2; row <= rowCount; row++)
                        {
                            Job job = new Job();
                            for (int col = 2; col <= colCount; col++)
                            {
                                string cellValue = worksheet.Cells[row, col].Text;
                                // Get Job's information
                                switch(worksheet.Cells[1, col].Text)
                                {
                                    case "JobName":
                                        job.JobName = cellValue;
                                        break;
                                    case "StartDate":
                                        job.StartDate = DateTime.Parse(cellValue);
                                        break;
                                    case "EndDate":
                                        job.EndDate = DateTime.Parse(cellValue);
                                        break;
                                    case "SalaryMin":
                                        job.SalaryMin = Convert.ToInt32(cellValue);
                                        break;
                                    case "SalaryMax":
                                        job.SalaryMax = Convert.ToInt32(cellValue);
                                        break;
                                    case "WorkingAddress":
                                        job.WorkingAddress = cellValue;
                                        break;
                                    case "Description":
                                        job.Description = cellValue;
                                        break;
                                    case "Skill":
                                        string[] skillsData = cellValue.Split(',');
                                        try
                                        {
                                            for(int i = 0;i< skillsData.Length; i++)
                                            {
                                                foreach(Skill skill in skills)
                                                {
                                                    if (skillsData[i].Trim().Equals(skill.SkillName))
                                                    {                                                       
                                                        job.Skills.Add(skill);
                                                    }
                                                }
                                            }
                                        }catch (Exception ex)
                                        {
                                            TempData["MessageType"] = "danger";
                                            TempData["Message"] = "Skill is not validate. Please check again";
                                            return RedirectToPage();

                                        }
                                        break;
                                    case "Benefit":
                                        string[] benefitsData = cellValue.Split(",");
                                        try
                                        {
                                            for(int i=0;i< benefitsData.Length; i++)
                                            {
                                                foreach(Benefit benefit in benefits)
                                                {
                                                    if (benefitsData[i].Trim().Equals(benefit.BenefitName))
                                                    {
                                                        job.Benefits.Add(benefit);
                                                    }
                                                }
                                            }
                                        }catch (Exception ex)
                                        {
                                            TempData["MessageType"] = "danger";
                                            TempData["Message"] = "Benefit is not validate. Please check again";
                                            return RedirectToPage();
                                        }
                                        break;
                                    case "Level":
                                        string[] levelsData = cellValue.Split(",");
                                        try
                                        {
                                            for (int i = 0; i < levelsData.Length; i++)
                                            {
                                                foreach (Level level in levels)
                                                {
                                                    if (levelsData[i].Trim().Equals(level.LevelName))
                                                    {
                                                        job.Levels.Add(level);
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            TempData["MessageType"] = "danger";
                                            TempData["Message"] = "Level is not validate. Please check again";
                                            return RedirectToPage();
                                        }
                                        break;
                                }
                            }
                            bool result = ValidateJob(job);
                            if(result)
                            {
                                job.Status = "Draft";
                                job.IsDeleted = false;
                                job.ModifiedBy = employee.FullName;
                                jobs.Add(job);
                            }
                            else
                            {
                                TempData["MessageType"] = "danger";
                                TempData["Message"] = "Job is not validate. Please check again";
                                return RedirectToPage();
                            }
                        }
                    }
                }
            }catch (Exception e)
            {
                TempData["MessageType"] = "danger";
                TempData["Message"] = "Job is not validate. Please check again";
                return RedirectToPage();
            }

            await _context.AddRangeAsync(jobs);
            _context.SaveChanges();
            TempData["MessageType"] = "success";
            TempData["Message"] = "Jobs have been imported";
            return RedirectToPage();
        }

        public bool ValidateJob(Job job)
        {
            var currentDate = DateTime.UtcNow;
            if(job.JobName == null || job.Skills == null || job.Benefits == null || job.Levels == null || job.WorkingAddress == null)
            {
                TempData["MessageType"] = "danger";
                TempData["Message"] = "Job's information cannot null";
                return false;
            }
            if (job.StartDate < currentDate)
            {
                TempData["MessageType"] = "danger";
                TempData["Message"] = "Start date must be greater than now";
                return false;
            }

            if (job.EndDate <= job.StartDate)
            {
                TempData["MessageType"] = "danger";
                TempData["Message"] = "End date must be greater than start date.";
                return false;
            }

            if (job.SalaryMax <= job.SalaryMin)
            {
                TempData["MessageType"] = "danger";
                TempData["Message"] = "Max salary must be greater than min salary.";
                return false;
            }
            return true;
        }

    }
}
