using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Attributes
{
    public class PasswordPolicyAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            var textValue = value.ToString();
            if (Common.Common.ValidatePassword(textValue)) return ValidationResult.Success;
            var errorMessage = FormatErrorMessage((validationContext.DisplayName));
            return new ValidationResult(errorMessage);
        }
    }
}