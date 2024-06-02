using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterviewManagement.Models
{
    public class Level
    {
        public Level()
        {
            Jobs = new HashSet<Job>();
            Candidates = new HashSet<Candidate>();
            Offers = new HashSet<Offer>();
        }
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? LevelName { get; set; }

        [InverseProperty("Levels")]
        public virtual ICollection<Job>? Jobs { get; set; }

        public virtual ICollection<Candidate>? Candidates { get; set; }

        public virtual ICollection<Offer>? Offers { get; set; }
    }
}
