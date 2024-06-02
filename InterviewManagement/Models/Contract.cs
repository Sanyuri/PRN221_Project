using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterviewManagement.Models
{
    public class Contract
    {
        public Contract()
        {
            Offers = new HashSet<Offer>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? ContractName { get; set; }

        public virtual ICollection<Offer>? Offers { get; set; }
    }
}
