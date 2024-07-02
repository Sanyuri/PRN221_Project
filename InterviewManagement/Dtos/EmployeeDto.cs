using InterviewManagement.DTOs;
using InterviewManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace InterviewManagement.Dtos
{
    public class EmployeeDto
    {
        public long Id { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }

        [Required]
        public string? FullName { get; set; }
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public DateTime Dob { get; set; }

        [Required]
        [StringLength(11, MinimumLength = 9, ErrorMessage = "Phone number must be between 9 and 11 characters long")]
        [RegularExpression(@"^\d{9,11}$", ErrorMessage = "Phone number must contain only numbers")]
        public string? PhoneNumber { get; set; }

        [Required]
        public string? Address { get; set; }

        [Required]
        public long RoleId { get; set; }
        [Required]
        public long DepartmentId { get; set; }
        public string? Note { get; set; }
        public string? Gender { get; set; }
        public string? Status { get; set; }


        public static EmployeeDto FromCandidate(Employee employee)
        {

            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee));
            }
            var dto = new EmployeeDto
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Email = employee.Email,
                Address = employee.Address,
                Dob = employee.dob,
                PhoneNumber = employee.PhoneNumber,
                RoleId = employee.Role.Id,
                DepartmentId = employee.Department.Id
            };

            return dto;
        }

    }
}
