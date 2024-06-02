using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterviewManagement.Models
{
    public class Benefit
    {
        public Benefit()
        {
            Jobs = new HashSet<Job>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? BenefitName { get; set; }

        [InverseProperty("Benefits")]
        public virtual ICollection<Job>? Jobs { get; set; }
    }
}
