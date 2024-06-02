using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterviewManagement.Models
{
    public class Skill
    {
        public Skill() {
            Candidates = new HashSet<Candidate>();
            Jobs = new HashSet<Job>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? SkillName { get; set; }

        [InverseProperty("Skills")]
        public virtual ICollection<Candidate>? Candidates { get; set; }

        [InverseProperty("Skills")]
        public virtual ICollection<Job>? Jobs { get; set; }
    }
}
