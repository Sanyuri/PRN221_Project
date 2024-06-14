using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterviewManagement.Models
{
    public class Candidate : User
    {
        public Candidate() {
            Schedules = new HashSet<Schedule>();
            Offers = new HashSet<Offer>();
            Skills = new HashSet<Skill>();
        }
        [Required]
        public string? CvLink { get; set; }

        [InverseProperty("Candidates")]
        public virtual ICollection<Skill>? Skills { get; set; }

        public virtual Employee? Employee { get; set; }

        [Required]
        public int ExpYear { get; set; }

        [Required]
        public DateTime? CreatedOn { get; set; }

        [Required]
        public string? ModifiedBy { get; set; }

        [ForeignKey("LevelId")]
        public virtual Level? Level { get; set; }

        [ForeignKey("HighestLevelId")]
        public virtual HighestLevel? HighestLevel { get; set; }

        [ForeignKey("PositionId")]
        public virtual Position? Position { get; set; }

        public string? Status { get; set; }

        public virtual ICollection<Schedule>? Schedules { get; set; }

        public virtual ICollection<Offer>? Offers { get; set; }
    }
}
