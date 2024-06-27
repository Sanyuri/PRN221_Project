﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InterviewManagement.Models;
using InterviewManagement.DTOs;
using InterviewManagement.Values;
using System.Collections.ObjectModel;

namespace InterviewManagement.Pages.candidate
{
    public class UpdateModel : PageModel
    {
        private readonly InterviewManagementContext _context;

        public UpdateModel(InterviewManagementContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CandidateDTO CandidateDTO { get; set; }

        public IDictionary<int, string> StatusList { get; } = StatusValue.CandidateStatus;
        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candidate = await _context.Candidate
                .Include(c => c.Position)
                .Include(c => c.Skills)
                .Include(c => c.Employee)
                .Include(c => c.HighestLevel)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (candidate == null)
            {
                return NotFound();
            }

            CandidateDTO = CandidateDTO.FromCandidate(candidate);

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
            long idChan = long.Parse(Request.Form["idChanfer"]);

            var candidateToUpdate = _context.Candidate.Find(idChan);


            if (candidateToUpdate == null)
            {
                return NotFound();
            }

            candidateToUpdate.FullName = CandidateDTO.FullName;
            candidateToUpdate.Email = CandidateDTO.Email;
            candidateToUpdate.dob = CandidateDTO.Dob;
            candidateToUpdate.PhoneNumber = CandidateDTO.PhoneNumber;
            candidateToUpdate.Address = CandidateDTO.Address;
            candidateToUpdate.Gender = CandidateDTO.Gender;
            candidateToUpdate.Note = CandidateDTO.Note;
            candidateToUpdate.Role = _context.Role.Find(CandidateDTO.RoleId);
            candidateToUpdate.CvLink = CandidateDTO.CvLink;
            candidateToUpdate.Status = CandidateDTO.Status;
            candidateToUpdate.ExpYear = CandidateDTO.ExpYear;
            candidateToUpdate.CreatedOn = CandidateDTO.CreatedOn;
            candidateToUpdate.ModifiedBy = CandidateDTO.ModifiedBy;
            candidateToUpdate.Position = _context.Position.Find(CandidateDTO.PositionId);
            candidateToUpdate.Employee = _context.Employee.Find(CandidateDTO.EmployeeId);
            candidateToUpdate.HighestLevel = _context.HighestLevel.Find(CandidateDTO.HighestLevelId);

            ICollection<Skill> skillsAdd = _context.Skill
                                 .Where(skill => CandidateDTO.SkillIds.Contains(skill.Id))
                                 .ToList();
            AddSkillsToCandidate(idChan, skillsAdd.Select(c=>c.Id).ToList());
             try
            {
                _context.Attach(candidateToUpdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CandidateExists(CandidateDTO.Id))
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

        private async Task SetViewDataAsync()
        {
            ViewData["positionList"] = new SelectList(await _context.Position.ToListAsync(), "Id", "PositionName");
            ViewData["levelList"] = new SelectList(await _context.HighestLevel.ToListAsync(), "Id", "Name");
            ViewData["employList"] = new SelectList(await _context.Employee.ToListAsync(), "Id", "FullName");
            ViewData["skillsList"] = new SelectList(await _context.Skill.ToListAsync(), "Id", "SkillName");
            ViewData["statusList"] = new SelectList(StatusList.ToDictionary(p => p.Key, p => p.Value), "Key", "Value");
        }

        private bool CandidateExists(long id)
        {
            return _context.Candidate.Any(e => e.Id == id);
        }
        public ICollection<Skill> GetSkillsForCandidate(long candidateId)
        {
            var candidate = _context.Candidate
                                    .Include(c => c.Skills) // Include skills to avoid lazy loading issues
                                    .FirstOrDefault(c => c.Id == candidateId);

            if (candidate == null)
            {
                Debug.WriteLine($"Candidate with ID {candidateId} not found.");
                return new List<Skill>(); // Return an empty list if the candidate is not found
            }

            return candidate.Skills.ToList();
        }
        public void AddSkillsToCandidate(long candidateId, List<int> skillIdsToAdd)
        {
            var candidate = _context.Candidate
                                    .Include(c => c.Skills)
                                    .FirstOrDefault(c => c.Id == candidateId);
            if (candidate == null)
            {
                Debug.WriteLine($"Candidate with ID {candidateId} not found.");
                return;
            }
            ICollection<Skill> candidateSkill = GetSkillsForCandidate(candidateId);
            if (!candidateSkill.Any())
            {
                foreach (var skillId in skillIdsToAdd)
                {
                    // Check if the skill exists in the system
                    var skill = _context.Skill.Find(skillId);
                    if (skill == null)
                    {
                        Debug.WriteLine($"Skill with ID {skillId} not found.");
                        continue;
                    }

                    // Add the skill to the candidate
                    candidate.Skills?.Add(skill);
                }
                // The collection is empty
                Debug.WriteLine("The candidate has no skills.");
            }
            else
            {
                var skillexist = candidateSkill.Select(s => s.Id).ToList();
                if (skillexist.Any())
                {
                    RemoveSkillsFromCandidate(candidateId, skillexist);
                    foreach (var skillId in skillIdsToAdd)
                    {
                        // Check if the skill exists in the system
                        var skill = _context.Skill.Find(skillId);
                        if (skill == null)
                        {
                            Debug.WriteLine($"Skill with ID {skillId} not found.");
                            continue;
                        }

                        // Add the skill to the candidate
                        candidate.Skills?.Add(skill);
                    }
                }


                _context.SaveChanges();
            }
        }
            public void RemoveSkillsFromCandidate(long candidateId, List<int> skillIdsToRemove)
            {
                var candidate = _context.Candidate
                                        .Include(c => c.Skills)
                                        .FirstOrDefault(c => c.Id == candidateId);

                if (candidate == null)
                {
                    Debug.WriteLine($"Candidate with ID {candidateId} not found.");
                    return;
                }

                var skillsToRemove = candidate.Skills?
                                              .Where(s => skillIdsToRemove.Contains(s.Id))
                                              .ToList();
            if (skillIdsToRemove.Any()) { 
                foreach (var skill in skillsToRemove)
                {
                    candidate.Skills?.Remove(skill);
                }

            }
                
                _context.SaveChanges();
            }
        }

    }
