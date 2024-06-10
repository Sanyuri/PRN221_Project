﻿using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterviewManagement.Models
{
    public class Role
    {
        public Role() { 
            Users = new HashSet<User>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? RoleName { get; set; }

        public virtual ICollection<User>? Users { get; set; }
    }
}
