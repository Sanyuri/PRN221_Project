using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using InterviewManagement.Models;
using InterviewManagement.Values;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.DTOs;
using System.Diagnostics;

namespace InterviewManagement.Pages.candidate
{
    public class CreateModel : PageModel
    {
        private readonly InterviewManagementContext _context;

        public CreateModel(InterviewManagementContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CandidateDTO CandidateDTO { get; set; }

        public IDictionary<int, string> StatusList { get; } = StatusValue.CandidateStatus;
        public async Task<IActionResult> OnGetAsync()
        {
           
            await SetViewDataAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    Debug.WriteLine($"Field: {entry.Key}, Error: {string.Join(", ", entry.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                await SetViewDataAsync();
                return Page();
            }

            var candidateToAdd = new Candidate();
            Debug.WriteLine(candidateToAdd, "a");


            if (candidateToAdd == null)
            {
                return NotFound();
            }

            candidateToAdd.FullName = CandidateDTO.FullName;
            candidateToAdd.Email = CandidateDTO.Email;
            candidateToAdd.dob = CandidateDTO.Dob;
            candidateToAdd.PhoneNumber = CandidateDTO.PhoneNumber;
            candidateToAdd.Address = CandidateDTO.Address;
            candidateToAdd.Gender = CandidateDTO.Gender;
            candidateToAdd.Note = CandidateDTO.Note;
            candidateToAdd.Role = _context.Role.Find(CandidateDTO.RoleId);
            candidateToAdd.CvLink = CandidateDTO.CvLink;
            candidateToAdd.Status = CandidateDTO.Status;
            candidateToAdd.ExpYear = CandidateDTO.ExpYear;
            candidateToAdd.CreatedOn = CandidateDTO.CreatedOn;
            candidateToAdd.ModifiedBy = CandidateDTO.ModifiedBy;
            candidateToAdd.Position = _context.Position.Find(CandidateDTO.PositionId);
            candidateToAdd.Employee = _context.Employee.Find(CandidateDTO.EmployeeId);
            candidateToAdd.HighestLevel = _context.HighestLevel.Find(CandidateDTO.HighestLevelId);

            ICollection<Skill> skillsAdd = _context.Skill
                                 .Where(skill => CandidateDTO.SkillIds.Contains(skill.Id))
                                 .ToList();
            candidateToAdd.Skills = skillsAdd;
            try
            {
                _context.Candidate.Add(candidateToAdd);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
               
                    return NotFound();
              
            }

            return RedirectToPage("./Index");
        }

        private async Task SetViewDataAsync()
        {
            ViewData["positionList"] = new SelectList(await _context.Position.ToListAsync(), "Id", "PositionName");
            ViewData["levelList"] = new SelectList(await _context.HighestLevel.ToListAsync(), "Id", "Name");
            ViewData["employList"] = new SelectList(await _context.Employee.ToListAsync(), "Id", "FullName");
            ViewData["skillsList"] = new SelectList(await _context.Skill.ToListAsync(), "Id", "SkillName");
            ViewData["statusList"] = new SelectList(StatusList.ToDictionary(p => p.Key, p => p.Value), "Key", "Value");
        }

      
    }
}

