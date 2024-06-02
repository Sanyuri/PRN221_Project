﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterviewManagement.Models
{
    public class Employee : User
    {
        public Employee()
        {
            Schedules = new HashSet<Schedule>();
            Offers = new HashSet<Offer>();
            Candidates = new HashSet<Candidate>();
        }

        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }
        public string? ResetPasswordToken { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public virtual ICollection<Schedule>? Schedules { get; set; }
        public virtual ICollection<Offer>? Offers { get; set; }
        public virtual ICollection<Candidate>? Candidates { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status? Status { get; set; }
    }
}
