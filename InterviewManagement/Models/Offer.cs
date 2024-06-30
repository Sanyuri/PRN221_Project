using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterviewManagement.Models
{
    public class Offer
    {
        public Offer() { 
            Employees = new HashSet<Employee>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime? ContractFrom { get; set; }

        [Required]
        public DateTime? ContractTo { get; set; }

        [Required]
        public DateTime? DueDate { get; set; }

        [Required]
        public double? Salary { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Note { get; set; }

        [ForeignKey("CandidateId")]
        public virtual Candidate? Candidate { get; set; }

        [ForeignKey("PositionId")]
        public virtual Position? Position { get; set; }

        [ForeignKey("ScheduleId")]
        public virtual Schedule? Schedule { get; set; }

        [ForeignKey("ContractId")]
        public virtual Contract? Contract { get; set; }

        [ForeignKey("StatusId")]
        public string? Status { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        [ForeignKey("LevelId")]
        public virtual Level? Level { get; set; }

        [InverseProperty("Offers")]
        public virtual ICollection<Employee>? Employees { get; set; }

    }
}
