using InterviewManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InterviewManagement.DTOs
{
    public class CandidateDTO
    {
        public long Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public DateTime Dob { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string? Note { get; set; }

        [Required]
        public long RoleId { get; set; }

        public string CvLink { get; set; }

        public long? EmployeeId { get; set; } // Nullable if Employee is not assigned

        public int ExpYear { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public int? HighestLevelId { get; set; }

        public int? PositionId { get; set; }

        public string Status { get; set; }

        public ICollection<int> SkillIds { get; set; } // Assuming SkillIds are stored as integers

        public ICollection<long> ScheduleIds { get; set; } // Assuming Schedule IDs are integers

        public ICollection<int> OfferIds { get; set; } // Assuming Offer IDs are integers

        public CandidateDTO()
        {
            SkillIds = new List<int>();
            ScheduleIds = new List<long>();
            OfferIds = new List<int>();
        }

        public static CandidateDTO FromCandidate(Candidate candidate)
        {
            if (candidate == null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            var dto = new CandidateDTO
            {
                Id = candidate.Id,
                FullName = candidate.FullName,
                Email = candidate.Email,
                Dob = candidate.dob,
                PhoneNumber = candidate.PhoneNumber,
                Address = candidate.Address,
                Gender = candidate.Gender,
                Note = candidate.Note,
                RoleId = candidate.Role?.Id ?? 0, // Null check for Role
                CvLink = candidate.CvLink,
                EmployeeId = candidate.Employee?.Id, // Null check for Employee
                ExpYear = candidate.ExpYear,
                CreatedOn = candidate.CreatedOn,
                ModifiedBy = candidate.ModifiedBy,
                HighestLevelId = candidate.HighestLevel?.Id, // Null check for HighestLevel
                PositionId = candidate.Position?.Id, // Null check for Position
                Status = candidate.Status,
                SkillIds = GetSkillIds(candidate.Skills), // Null check for Skills
                ScheduleIds = GetScheduleIds(candidate.Schedules), // Null check for Schedules
                OfferIds = GetOfferIds(candidate.Offers) // Null check for Offers
            };

            return dto;
        }


        private static ICollection<int> GetSkillIds(ICollection<Skill> skills)
        {
            var skillIds = new List<int>();
            foreach (var skill in skills)
            {
                skillIds.Add(skill.Id);
            }
            return skillIds;
        }

        private static ICollection<long> GetScheduleIds(ICollection<Schedule> schedules)
        {
            var scheduleIds = new List<long>();
            foreach (var s in schedules)
            {
                scheduleIds.Add(s.Id);
            }
            return scheduleIds;
        }

        private static ICollection<int> GetOfferIds(ICollection<Offer> offers)
        {
            var offerIds = new List<int>();
            foreach (var offer in offers)
            {
                offerIds.Add(offer.Id);
            }
            return offerIds;
        }
    }
}
