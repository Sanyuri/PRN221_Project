using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterviewManagement.Models
{
    [NotMapped]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? FullName { get; set; }

        [Required]
        public DateTime dob { get; set; }

        [Required]
        [StringLength(13)]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress]
        [Column("email")]
        public string? Email { get; set; }

        [Required]
        public string? Address {  get; set; }

        [Required]
        public string? Gender { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Note {  get; set; }

        [Required]
        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
    }
}
