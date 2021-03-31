using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class ReceiverDetailsViewModel
    {
        public const string BindProperty = " PreviousReceivers ,ReceiverFirstName ,ReceiverMiddleName , ReceiverLastName,ReceiverCity  , ReceiverCountry  ,  ReceiverPhoneNumber" +
            " , ReceiverEmailAddress ,CreatedBY , IsDeleted ,CountryPhoneCode ";
        public string PreviousReceivers { get; set; }


        [Required(ErrorMessage = "First Name Is Required")]
        [Display(Name = "First Name")]
        [RegularExpression(@"[a-z A-Z]+$", ErrorMessage = "Use letters only please.")]
        public string ReceiverFirstName { get; set; }


        [Display(Name = "Middle Name")]
        [RegularExpression(@"[a-z A-Z]+$", ErrorMessage = "Use letters only please.")]
        public string ReceiverMiddleName { get; set; }

        [Required(ErrorMessage = "Last Name Is Required")]
        [Display(Name = "Last Name")]
        [RegularExpression(@"[a-z A-Z]+$", ErrorMessage = "Use letters only please.")]
        public string ReceiverLastName { get; set; }

        [Required(ErrorMessage = "City Is Required")]
        [Display(Name = "City")]
        public string ReceiverCity { get; set; }

        [Required(ErrorMessage = "Country Is Required")]
        [Display(Name = "Country")]
        public string ReceiverCountry { get; set; }

        [Required(ErrorMessage = "Phone Number Is Required")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Use number only please.")]
        public string ReceiverPhoneNumber { get; set; }


        [EmailAddress]
        //[Required(ErrorMessage = "Email Address Is Required")]
        [Display(Name = "Email")]
        public string ReceiverEmailAddress { get; set; }

       
        public string CreatedBY { get; set; }
        public bool IsDeleted { get; set; }
        public string CountryPhoneCode { get; set; }

    }
}