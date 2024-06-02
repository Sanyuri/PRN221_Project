using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterviewManagement.Models
{
    public class Status
    {
        public Status() {
            Employees = new HashSet<Employee>();
            Candidates = new HashSet<Candidate>();
            Jobs = new HashSet<Job>();
            Offers = new HashSet<Offer>();
            Schedules = new HashSet<Schedule>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? StatusName { get; set; }

        public virtual ICollection<Employee>? Employees { get; set; }

        public virtual ICollection<Candidate>? Candidates { get; set; }

        public virtual ICollection<Job>? Jobs { get; set; }

        public virtual ICollection<Offer>? Offers { get; set; }

        public virtual ICollection<Schedule>? Schedules { get; set; }
    }
}
