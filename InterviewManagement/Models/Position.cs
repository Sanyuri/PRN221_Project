using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterviewManagement.Models
{
    public class Position
    {
        public Position()
        {
            Offers = new HashSet<Offer>();
            Candidates = new HashSet<Candidate>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? PositionName { get; set; }

        public virtual ICollection<Offer>? Offers { get; set; }

        public virtual ICollection<Candidate>? Candidates { get; set; }
    }
}
