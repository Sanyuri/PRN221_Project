using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;
using InterviewManagement.Values;
using InterviewManagement.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Diagnostics;
using OfficeOpenXml;

namespace InterviewManagement.Pages.candidate
{
    [Authorize(Policy = "Employee")]

    public class IndexModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;

        public IndexModel(InterviewManagement.Models.InterviewManagementContext context)
        {
            _context = context;
        }
        [BindProperty]
        public IFormFile ExcelFile { get; set; } = default!;

        public IList<Candidate> Candidate { get; set; } = default!;
        public IDictionary<int, string> status { get; } = StatusValue.CandidateStatus;
        public string SearchTerm { get; set; } = string.Empty;
        public string StatusFilter { get; set; } = "All";

        // Pagination properties
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;

        public async Task OnGetAsync(int? pageNumber, string searchTerm, string statusFilter)
        {
            CurrentPage = pageNumber ?? 1;
            SearchTerm = searchTerm;
            StatusFilter = statusFilter;
            long accountId = long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value);
            var sessionRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var user = await _context.Employee.Include(c => c.Role).Where(c => c.Role.RoleName == sessionRole).FirstOrDefaultAsync();

            var candidatesQuery = _context.Candidate
                .Include(c => c.Employee)
                .Include(c => c.Position)
                .Where(c => c.IsDeleted == false)
                .AsQueryable();

            if (user.Role?.RoleName == "Interviewer")
            {
                candidatesQuery = candidatesQuery.Include(s => s.Schedules).ThenInclude(e => e.Employees).Where(c => c.Schedules.Any(s => s.Employees.Any(e => e.Id == accountId)));
            }

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                candidatesQuery = candidatesQuery.Where(c =>
                    (c.FullName != null && c.FullName.Contains(SearchTerm)) ||
                    (c.Email != null && c.Email.Contains(SearchTerm)));
            }

            if (!string.IsNullOrEmpty(StatusFilter) && StatusFilter != "All")
            {
                candidatesQuery = candidatesQuery.Where(c => c.Status == StatusFilter);
            }

            var totalCandidates = await candidatesQuery.CountAsync();
            TotalPages = (int)Math.Ceiling(totalCandidates / (double)PageSize);

            Candidate = await candidatesQuery
                .OrderBy(c => Convert.ToInt32(c.Status))
                .ThenBy(c => c.CreatedOn)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
            if (user.Role?.RoleName == "Recruiter")
            {
                IDictionary<int, string> a = new Dictionary<int, string>()
                 {
            { 1, "Waiting for interview" },
            { 2, "Waiting for approval" },
            { 3, "Waiting for response" },
            { 4, "Open" },
            { 8, "Accepted offer" },
            { 9, "Declined offer" },
            { 10, "Cancelled offer" },
            { 13, "Banned" }
                 };
                ViewData["statuss"] = a;
            }
            else if (user.Role?.RoleName == "Manager")
            {
                IDictionary<int, string> a = new Dictionary<int, string>()
                 {
            { 8, "Accepted offer" },
            { 9, "Declined offer" },
                 };
                ViewData["statuss"] = a;
            }else if(user.Role?.RoleName == "Interviewer")
            {
                IDictionary<int, string> a = new Dictionary<int, string>()
                 {
                       { 5, "Passed" },
                       { 11, "Failed interview" },
                       { 12, "Cancelled interview" },

                 };
                ViewData["statuss"] = a;
            }
            else
            {
                ViewData["statuss"] = status;

            }
            ViewData["status"] = status;

            ViewData["User"] = user?.Role?.RoleName;
        }

        public IActionResult OnPost()
        {
            string searchTerm = Request.Form["searchTerm"];
            string statusFilter = Request.Form["filter"];

            return RedirectToPage(new { pageNumber = 1, searchTerm, statusFilter });
        }

        public async Task<IActionResult> OnPostImportCandidate()
        {
            if (ExcelFile == null || ExcelFile.Length == 0)
            {
                TempData["MessageType"] = "danger";
                TempData["Message"] = "File is not existed";
                return RedirectToPage();
            }
            List<Candidate> candidates = new List<Candidate>();
            try
            {
                using (var stream = new MemoryStream())
                {
                    await ExcelFile.CopyToAsync(stream);
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[1];
                        int rowCount = worksheet.Dimension.Rows;
                        int colCount = worksheet.Dimension.Columns;

                        List<HighestLevel> highestLevels = await _context.HighestLevel.ToListAsync();
                        List<Position> positions = await _context.Position.ToListAsync();
                        List<Skill> skills = await _context.Skill.ToListAsync();
                        List<Employee> employees = await _context.Employee.ToListAsync();

                        var AccountId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                        Employee? modifier = _context.Employee.Find(long.Parse(AccountId));

                        for (int row = 2; row <= rowCount; row++)
                        {
                            Candidate candidate = new Candidate();
                            for (int col = 2; col <= colCount; col++)
                            {
                                string cellValue = worksheet.Cells[row, col].Text;
                                // Get Candidate's information
                                switch (worksheet.Cells[1, col].Text)
                                {
                                    case "FullName":
                                        candidate.FullName = cellValue;
                                        break;
                                    case "Email":
                                        candidate.Email = cellValue;
                                        break;
                                    case "Dob":
                                        candidate.dob = DateTime.Parse(cellValue);
                                        break;
                                    case "Address":
                                        candidate.Address = cellValue;
                                        break;
                                    case "PhoneNumber":
                                        candidate.PhoneNumber = cellValue;
                                        break;
                                    case "Gender":
                                        candidate.Gender = cellValue;
                                        break;
                                    case "HighestLevel":
                                        foreach (HighestLevel highestLevel in highestLevels)
                                        {
                                            if (highestLevel.Name.Equals(cellValue))
                                            {
                                                candidate.HighestLevel = highestLevel;
                                            }
                                        }
                                        break;
                                    case "Note":
                                        candidate.Note = cellValue;
                                        break;
                                    case "Position":
                                        foreach (Position position in positions)
                                        {
                                            if (position.PositionName.Equals(cellValue))
                                            {
                                                candidate.Position = position;
                                            }
                                        }
                                        break;
                                    case "Skill":
                                        string[] skillsData = cellValue.Split(',');
                                        try
                                        {
                                            for (int i = 0; i < skillsData.Length; i++)
                                            {
                                                foreach (Skill skill in skills)
                                                {
                                                    if (skillsData[i].Trim().Equals(skill.SkillName))
                                                    {
                                                        candidate.Skills.Add(skill);
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            TempData["MessageType"] = "danger";
                                            TempData["Message"] = "Skill is not validate. Please check again";
                                            return RedirectToPage();

                                        }
                                        break;
                                    case "ExpYear":
                                        candidate.ExpYear = Convert.ToInt32(cellValue);
                                        break;
                                    case "Recruiter":
                                        foreach (Employee employee in employees)
                                        {
                                            if (employee.FullName.Equals(cellValue))
                                            {
                                                candidate.Employee = employee;
                                            }
                                        }
                                        break;
                                }
                            }
                            candidate.ModifiedBy = modifier.FullName;
                            candidate.CreatedOn = DateTime.Now;
                            candidate.Role = _context.Role.Where(r => r.RoleName.Equals("Candidate")).FirstOrDefault();
                            candidate.Status = "4";
                            candidates.Add(candidate);
                        }
                    }
                }

                _context.AddRange(candidates);
                await _context.SaveChangesAsync();
                TempData["MessageType"] = "success";
                TempData["Message"] = "Candidates has been imported";
                return RedirectToPage();
            }
            catch (Exception e)
            {
                TempData["MessageType"] = "danger";
                TempData["Message"] = "Candidate is not validate. Please check again";
                return RedirectToPage();
            }
        }
    }
}
