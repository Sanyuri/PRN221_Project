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

        [Required(ErrorMessage ="ContractFrom is required")]
        public DateTime? ContractFrom { get; set; }

        [Required(ErrorMessage ="ContractTo is required")]
        public DateTime? ContractTo { get; set; }

        [Required(ErrorMessage = "DueDate is required")]
        public DateTime? DueDate { get; set; }

        [Required(ErrorMessage = "Salary is required")]
        public double? Salary { get; set; }

        public bool IsDeleted { get; set; }


        [Column(TypeName = "nvarchar(max)")]
        public string? Note { get; set; }

        public string? Status { get; set; }

        [Required(ErrorMessage = "Candidate is required")]
        public long? CandidateId { get; set; }
        [Required(ErrorMessage = "Position is required")]
        public int? PositionId { get; set; }
        [Required(ErrorMessage = "Schedule is required")]
        public long? ScheduleId { get; set; }
        [Required(ErrorMessage = "Contract is required")]
        public long? ContractId { get; set; }
        [Required(ErrorMessage = "Department is required")]
        public long? DepartmentId { get; set; }
        [Required(ErrorMessage = "Level is required")]
        public int? LevelId { get; set; }


        [ForeignKey("CandidateId")]
        public virtual Candidate? Candidate { get; set; }

        [ForeignKey("PositionId")]
        public virtual Position? Position { get; set; }

        [ForeignKey("ScheduleId")]
        public virtual Schedule? Schedule { get; set; }

        [ForeignKey("ContractId")]
        public virtual Contract? Contract { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        [ForeignKey("LevelId")]
        public virtual Level? Level { get; set; }

        [InverseProperty("Offers")]
        public virtual ICollection<Employee>? Employees { get; set; }

    }
}
