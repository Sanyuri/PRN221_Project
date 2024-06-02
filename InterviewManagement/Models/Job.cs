using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterviewManagement.Models
{
    public class Job
    {
        public Job() {
            Skills = new HashSet<Skill>();
            Benefits = new HashSet<Benefit>();
            Levels = new HashSet<Level>();
            Schedules = new HashSet<Schedule>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? JobName { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }

        [Required]
        public double? SalaryMin { get; set; }

        [Required]
        public DateTime? EndDate { get; set; }

        [Required]
        public double SalaryMax { get; set; }

        [Required]
        public string? WorkingAddress { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        [Required]
        public string? ModifiedBy { get; set; }


        [Column(TypeName = "nvarchar(max)")]
        public string? Description { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status? Status {  get; set; }

        [InverseProperty("Jobs")]
        public virtual ICollection<Skill>? Skills { get; set; }

        [InverseProperty("Jobs")]
        public virtual ICollection<Benefit>? Benefits { get; set; }

        [InverseProperty("Jobs")]
        public virtual ICollection<Level>? Levels { get; set; }

        public virtual ICollection<Schedule>? Schedules { get; set; }

    }
}
