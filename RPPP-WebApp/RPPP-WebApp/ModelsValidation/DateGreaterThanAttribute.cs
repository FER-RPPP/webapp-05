using System;
using System.ComponentModel.DataAnnotations;


namespace RPPP_WebApp.ModelsValidation
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var comparisonProperty = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (comparisonProperty == null)
            {
                throw new ArgumentException($"Property {_comparisonProperty} not found.");
            }

            var startDate = (DateTime?)value;
            var endDate = (DateTime?)comparisonProperty.GetValue(validationContext.ObjectInstance);

            // If either date is null, the validation is considered successful
            if (!startDate.HasValue || !endDate.HasValue)
            {
                return ValidationResult.Success;
            }

            if (startDate <= endDate)
            {
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} must be greater than {_comparisonProperty}.");
            }

            return ValidationResult.Success;
        }
    }

}



