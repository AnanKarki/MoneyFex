using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class KiiPayReceiverDetailsInformationViewModel
    {

        public const string BindProperty = "Country , PreviousMobileNumber ,MobileNo,CountryPhoneCode , ReceiverFullName ,ReasonForTransfer";

        [Required(ErrorMessage = "Select Country")]
        public string Country { get; set; }


        public string PreviousMobileNumber { get; set; }
        [Required(ErrorMessage = "Enter Mobile Number")]
        public string MobileNo { get; set; }
        public string CountryPhoneCode { get; set; }
        [Required(ErrorMessage = "Enter Receiver Full Name")]

        public string ReceiverFullName { get; set; }
        [Required(ErrorMessage = "select Reason")]
        public ReasonForTransfer ReasonForTransfer { get; set; }


    }
}