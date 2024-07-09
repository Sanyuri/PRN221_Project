using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;
using InterviewManagement.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging;
using System.ComponentModel.DataAnnotations;
using InterviewManagement.Values;
using OfficeOpenXml;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Authorization;

namespace InterviewManagement.Pages.ims.recruitment.com.Offers
{
    [Authorize(Policy ="Offer")]
    public class IndexModel : PageModel
    {
        private readonly InterviewManagement.Models.InterviewManagementContext _context;

        public IndexModel(InterviewManagement.Models.InterviewManagementContext context)
        {
            _context = context;
        }
        [BindProperty]
        public DateTime ContractFrom { get; set; } = default!;

        [BindProperty]
        public DateTime ContractTo { get; set; } = default!;

        [BindProperty]
        public long ApproverId { get; set; } = default!;

        [BindProperty]
        public long RecruiterId { get; set; } = default!;

        [BindProperty]
        public int OfferId { get; set; } = default!;
        public IList<Offer> Offers { get; set; } = default!;

        [BindProperty]
        public Offer Offer { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public long? DepartmentFilter { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; } = default!;

        public async Task OnGetAsync(int? page)
        {
            if (_context.Offer != null)
            {

                ViewData["Candidate"] = new SelectList(_context.Candidate, "Id", "FullName");
                ViewData["Contract"] = new SelectList(_context.Contract, "Id", "ContractName");
                ViewData["Position"] = new SelectList(_context.Position, "Id", "PositionName");
                ViewData["Level"] = new SelectList(_context.Level, "Id", "LevelName");
                ViewData["InterviewInfo"] = new SelectList(_context.Schedule, "Id", "ScheduleName");
                ViewData["Department"] = new SelectList(_context.Department, "Id", "DepertmentName");
                ViewData["Approver"] = new SelectList(_context.Employee.Where(r => r.Role.RoleName.Equals("Manager")), "Id", "FullName");
                ViewData["Recruiter"] = new SelectList(_context.Employee.Where(r => r.Role.RoleName.Equals("Recruiter")), "Id", "FullName");
                ViewData["ScheduleNote"] = _context.Schedule.ToDictionary(s => s.Id, s => s.Note);

                Offers = await _context.Offer
                                            .Include(o => o.Candidate)
                                            .Include(o => o.Department)
                                            .Include(o => o.Employees).ThenInclude(r => r.Role)
                                            .Include(o => o.Level)
                                            .Include(o => o.Schedule)
                                            .Include(o => o.Contract)
                                            .Where(o => o.IsDeleted == false)
                                            .Where(o => !DepartmentFilter.HasValue || o.DepartmentId == DepartmentFilter)
                                            .Where(o => StatusFilter == null || o.Status.Equals(StatusFilter))
                                            .ToListAsync();
                //Pageing               
            }
        }
        public async Task<IActionResult> OnPostAddOffer()
        {
            if (!ModelState.IsValid || _context.Offer == null || Offer == null)
            {
                return RedirectToPage("./Index");
            }
            Employee Approver = _context.Employee.Find(ApproverId);
            Employee Recruiter = _context.Employee.Find(RecruiterId);

            Offer.Employees.Add(Approver);
            Offer.Employees.Add(Recruiter);

            Offer.Status = "Waiting for approval";
            Offer.IsDeleted = false;
            _context.AddAsync(Offer);
            //change Candidate's status
            Candidate candidate = _context.Candidate.Find(Offer.CandidateId);
            candidate.Status = "2";

            _context.Update(candidate);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostUpdateOffer()
        {
            if (!ModelState.IsValid || _context.Offer == null || Offer == null)
            {
                return RedirectToPage("./Index");
            }
            Offer.Employees = new List<Employee>();
            Employee Approver = _context.Employee.Find(ApproverId);
            Employee Recruiter = _context.Employee.Find(RecruiterId);
            Offer.Employees.Add(Approver);
            Offer.Employees.Add(Recruiter);
            _context.Attach(Offer).State = EntityState.Modified;
            //change Candidate's status
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostUpdateOfferStatus()
        {
            if (!ModelState.IsValid || _context.Offer == null || Offer == null)
            {
                return RedirectToPage("./Index");
            }
            //change Candidate's status
            if (Offer.Status.Equals("Waiting for Response"))
            {
                Candidate candidate = _context.Candidate.Find(Offer.CandidateId);
                candidate.Status = "3";
                _context.Update(candidate);
            }
            if (Offer.Status.Equals("Accepted"))
            {
                Candidate candidate = _context.Candidate.Find(Offer.CandidateId);
                candidate.Status = "8";
                _context.Update(candidate);
            }
            if (Offer.Status.Equals("Declined"))
            {
                Candidate candidate = _context.Candidate.Find(Offer.CandidateId);
                candidate.Status = "9";
                _context.Update(candidate);
            }
            _context.Offer.Attach(Offer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostCancelOffer()
        {
            if (OfferId == null || _context.Offer == null)
            {
                return RedirectToPage("./Index");
            }
            Offer offer = _context.Offer.Find(OfferId);
            offer.IsDeleted = true;
            _context.Update(offer);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostExportOffer()
        {
            if (ContractFrom > ContractTo || _context.Offer == null)
            {
                return RedirectToPage("./Index");
            }
            IList<Offer> OffersExport = await _context.Offer
                               .Include(o => o.Candidate)
                               .Include(o => o.Department)
                               .Include(o => o.Employees).ThenInclude(r => r.Role)
                               .Include(o => o.Level)
                               .Include(o => o.Schedule)
                               .Include(o => o.Contract)
                               .Where(o => (ContractFrom <= o.ContractFrom && ContractTo >= o.ContractFrom))
                               .ToListAsync();

            if (OffersExport != null)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Offer");

                    // Đặt tiêu đề cột
                    worksheet.Cells[1, 1].Value = "Id";
                    worksheet.Cells[1, 2].Value = "Contract From";
                    worksheet.Cells[1, 3].Value = "Contract To";
                    worksheet.Cells[1, 4].Value = "Due Date";
                    worksheet.Cells[1, 5].Value = "Salary";
                    worksheet.Cells[1, 6].Value = "Is Deleted";
                    worksheet.Cells[1, 7].Value = "Note";
                    worksheet.Cells[1, 8].Value = "Status";
                    worksheet.Cells[1, 9].Value = "Candidate";
                    worksheet.Cells[1, 10].Value = "Position";
                    worksheet.Cells[1, 11].Value = "Schedule";
                    worksheet.Cells[1, 12].Value = "Contract";
                    worksheet.Cells[1, 13].Value = "Department";
                    worksheet.Cells[1, 14].Value = "Level";

                    // Điền dữ liệu vào worksheet
                    int row = 2;
                    foreach (var offer in OffersExport)
                    {
                        worksheet.Cells[row, 1].Value = offer.Id;
                        worksheet.Cells[row, 2].Value = offer.ContractFrom?.ToString("yyyy-MM-dd");
                        worksheet.Cells[row, 3].Value = offer.ContractTo?.ToString("yyyy-MM-dd");
                        worksheet.Cells[row, 4].Value = offer.DueDate?.ToString("yyyy-MM-dd");
                        worksheet.Cells[row, 5].Value = offer.Salary;
                        worksheet.Cells[row, 6].Value = offer.IsDeleted;
                        worksheet.Cells[row, 7].Value = offer.Note;
                        worksheet.Cells[row, 8].Value = offer.Status;
                        worksheet.Cells[row, 9].Value = offer.Candidate?.FullName;
                        worksheet.Cells[row, 10].Value = offer.Position?.PositionName;
                        worksheet.Cells[row, 11].Value = offer.Schedule?.ScheduleName;
                        worksheet.Cells[row, 12].Value = offer.Contract?.ContractName;
                        worksheet.Cells[row, 13].Value = offer.Department?.DepertmentName;
                        worksheet.Cells[row, 14].Value = offer.Level?.LevelName;
                        row++;
                    }

                    // Lưu tệp Excel
                    string fileName = $"Offerlist-{ContractFrom.ToString("dd-MM-yyyy")}_{ContractTo.ToString("dd-MM-yyyy")}.xlsx";
                    var stream = new MemoryStream();
                    package.SaveAs(stream);
                    stream.Position = 0;

                    // Trả về tệp Excel để tải xuống
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            return RedirectToPage("./Index");
        }
    }
}
