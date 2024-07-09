using System.ComponentModel.DataAnnotations;

namespace InterviewManagement.CustomValidationAttributes
{
    public class ContractValidateAttribute : ValidationAttribute
    {
        private readonly string _startDatePropertyName;

        public ContractValidateAttribute(string startDatePropertyName)
        {
            _startDatePropertyName = startDatePropertyName;
            ErrorMessage = "{0} must be greater than {1}.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var startDateProperty = validationContext.ObjectType.GetProperty(_startDatePropertyName);

            if (startDateProperty == null)
            {
                throw new ArgumentException("Invalid property name");
            }

            var startDateValue = (DateTime?)startDateProperty.GetValue(validationContext.ObjectInstance);
            var endDateValue = (DateTime?)value;

            if (endDateValue.HasValue && startDateValue.HasValue && endDateValue.Value.Date <= startDateValue.Value.Date)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}
