using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    /// <summary>
    /// Virtual Account Registration 
    /// </summary>
    public class TopUpCardUserDetailsModelView
    {
        public const string BindProperty = "TopUpUserFirstName,TopUpUserMiddleName,TopUpUserLastName,TopUpUserDateOfBirth,AgeVerification";
        [Required(ErrorMessage = "Enter First Name")]
        [Display(Name = "First Name")]
        [RegularExpression(@"[a-z A-Z]+$", ErrorMessage = "Use letters only please.")]
        public string TopUpUserFirstName { get; set; }

        [Display(Name = "Middle Name")]
        [RegularExpression(@"[a-z A-Z]+$", ErrorMessage = "Use letters only please.")]
        public string TopUpUserMiddleName { get; set; }

        [Required(ErrorMessage = "Enter Last Name")]
        [Display(Name = "Last Name")]
        [RegularExpression(@"[a-z A-Z]+$", ErrorMessage = "Use letters only please.")]
        public string TopUpUserLastName { get; set; }

        [Required(ErrorMessage = "Enter Date of Birth")]
        [Display(Name = "Date of Birth")]
        public DateTime TopUpUserDateOfBirth { get; set; }
        
        public bool AgeVerification { get; set; }
    }
}