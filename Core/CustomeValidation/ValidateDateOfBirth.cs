using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CustomeValidation
{
    internal sealed  class ValidateDateOfBirth:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var result= DateOnly.TryParse(value.ToString(),out DateOnly date);
            if(!result)
                  return new ValidationResult("Invalid Date Of Birth.");



            var today = DateOnly.FromDateTime(DateTime.Today);

            int manyYears = today.Year - date.Year;

            if (date.AddYears(manyYears) > today)
                manyYears--;




            if(manyYears < 18)
            {
                return new ValidationResult("you should be at least 18 years old.");
            }
            return ValidationResult.Success;

        }
    }
}
