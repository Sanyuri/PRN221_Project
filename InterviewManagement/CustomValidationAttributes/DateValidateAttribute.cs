
using System.ComponentModel.DataAnnotations;

namespace InterviewManagement.CustomValidationAttributes
{
    internal class DateValidateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime date)
            {
                return !(date.Date < DateTime.Today.Date);
            }
            return false;
        }
    }
}