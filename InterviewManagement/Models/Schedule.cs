using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterviewManagement.Models
{
    public class Schedule
    {
        public Schedule()
        {
            Employees = new HashSet<Employee>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? ScheduleName { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Note { get; set; }

        [Required]
        public DateTime? ScheduleTime { get; set; }

        [Required]
        public string? MeetingURL { get; set; }

        [Required]
        public string? Result { get; set; }

        [Required]
        public string? Location { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        [Required]
        public string? ModifiedBy { get; set; }

        public string? Status { get; set; }
        [ForeignKey("CandidateId")]
        public virtual Candidate? Candidate { get; set; }
        [ForeignKey("JobId")]
        public virtual Job? Job { get; set; }

        [InverseProperty("Schedules")]
        public virtual ICollection<Employee>? Employees { get; set; }
    }
}
