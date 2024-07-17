using InterviewManagement.DTOs;
using InterviewManagement.Models;
using InterviewManagement.Values;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace InterviewManagement.Pages.ims.recruitment.com.candidate
{
    [Authorize(Policy = "Employee")]

    public class EditModel : PageModel
    {
        private readonly InterviewManagementContext _context;

        public EditModel(InterviewManagementContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CandidateDTO CandidateDTO { get; set; }
        [BindProperty]
        public IFormFile? CvFile { get; set; }
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
            if (candidate.CvLink != null)
            {
                try {
                    var absolutePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", candidate.CvLink.TrimStart('/'));

                
                    var relativePath = Path.Combine("uploads", candidate.CvLink.TrimStart('/'));
                    if (System.IO.File.Exists(absolutePath))
                    {
                        Debug.WriteLine(relativePath);
                        ViewData["filePath"] = $"/{relativePath}"; // Use a relative URL for the web
                        ViewData["filename"] = Path.GetFileNameWithoutExtension(candidate.CvLink);
                    }
                } catch (Exception e) { }
              
             

            }
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
            try {
                if (CvFile != null)
                {
                    var fileName = Path.GetFileNameWithoutExtension(CvFile.FileName);
                    var extension = Path.GetExtension(CvFile.FileName);
                    var uniqueFileName = fileName + "_" + CandidateDTO.Email + "_" + DateTime.Now.ToString("dd-MM-yyyy") + extension;
                    var filePath = Path.Combine("wwwroot", "uploads", uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await CvFile.CopyToAsync(stream);
                    }

                    CandidateDTO.CvLink = uniqueFileName;
                }
            }
            catch(Exception e) { }
            

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
            if(CvFile != null)
            {
                candidateToUpdate.CvLink = CandidateDTO.CvLink;
            }
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
            AddSkillsToCandidate(idChan, skillsAdd.Select(c => c.Id).ToList());
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
            ViewData["employList"] = new SelectList(await _context.Employee.Include(c=>c.Role).Where(c=>c.Role.RoleName== "Recruiter").ToListAsync(), "Id", "FullName");
            ViewData["skillsList"] = new SelectList(await _context.Skill.ToListAsync(), "Id", "SkillName");

            var sessionRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var user = await _context.Employee.Include(c => c.Role).Where(c => c.Role.RoleName == sessionRole).FirstOrDefaultAsync();
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
                ViewData["statusList"] = new SelectList(a.ToDictionary(p => p.Key, p => p.Value), "Key", "Value");
            }
            else if (user.Role?.RoleName == "Manager")
            {
                IDictionary<int, string> a = new Dictionary<int, string>()
                 {
            { 8, "Accepted offer" },
            { 9, "Declined offer" },
                 };
                ViewData["statusList"] = new SelectList(a.ToDictionary(p => p.Key, p => p.Value), "Key", "Value");
            }
            else if (user.Role?.RoleName == "Interviewer")
            {
                IDictionary<int, string> a = new Dictionary<int, string>()
                 {
                       { 5, "Passed" },
                       { 11, "Failed interview" },
                       { 12, "Cancelled interview" },

                 };
                ViewData["statusList"] = new SelectList(a.ToDictionary(p => p.Key, p => p.Value), "Key", "Value");
            }
            else
            {
                ViewData["statusList"] = new SelectList(StatusList.ToDictionary(p => p.Key, p => p.Value), "Key", "Value");

            }
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
            if (skillIdsToRemove.Any())
            {
                foreach (var skill in skillsToRemove)
                {
                    candidate.Skills?.Remove(skill);
                }

            }

            _context.SaveChanges();
        }
    }
}
