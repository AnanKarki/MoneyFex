using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class ReceiversDetailsViewModel
    {

        public const string BindProperty = "ReceiverFirstName,ReceiverMiddleName,ReceiverLastName,ReceiverCity,ReceiverCountry,ReceiverPhoneNumber" +
            ",ReceiverEmailAddress,CreatedDate,CreatedBY,IsDeleted,CountryPhoneCode";
        [Required(ErrorMessage = "Enter First Name")]
        [Display(Name = "First Name")]
        [RegularExpression(@"[a-z A-Z]+$", ErrorMessage = "Use letters only please.")]
        public string ReceiverFirstName { get; set; }


        [Display(Name = "Middle Name")]
        [RegularExpression(@"[a-z A-Z]+$", ErrorMessage = "Use letters only please.")]
        public string ReceiverMiddleName { get; set; }

        [Required(ErrorMessage = "Enter Last Name")]
        [Display(Name = "Last Name")]
        [RegularExpression(@"[a-z A-Z]+$", ErrorMessage = "Use letters only please.")]
        public string ReceiverLastName { get; set; }

        [Required(ErrorMessage = "Enter City")]
        [Display(Name = "City")]
        public string ReceiverCity { get; set; }

        [Required(ErrorMessage = "Enter Country")]
        [Display(Name = "Country")]
        public string ReceiverCountry { get; set; }

        [Required(ErrorMessage = "Enter Phone Number")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Use number only please.")]
        public string ReceiverPhoneNumber { get; set; }
        

        [EmailAddress]
        [Required(ErrorMessage = "Enter Email Address")]
        [Display(Name = "Email")]
        public string ReceiverEmailAddress { get; set; }

        private DateTime _date = DateTime.UtcNow;
        public DateTime CreatedDate
        {
            get { return _date; }
            set { _date = value; }
        }
        public string CreatedBY { get; set; }
        public bool IsDeleted { get; set; }
        public string CountryPhoneCode { get; set; }
    }
}