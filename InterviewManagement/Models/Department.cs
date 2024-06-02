using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterviewManagement.Models
{
    public class Department
    {
        public Department() {
            Offers = new HashSet<Offer>();
            Employees = new HashSet<Employee>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? DepertmentName { get; set; }

        public virtual ICollection<Offer>? Offers { get; set; }

        public virtual ICollection<Employee>? Employees { get; set; }
    }
}
