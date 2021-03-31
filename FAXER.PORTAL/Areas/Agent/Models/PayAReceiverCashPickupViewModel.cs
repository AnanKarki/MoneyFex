using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class PayAReceiverCashPickupViewModel
    {
        public const string BindProperty = "Id , FirstName ,MiddleName,LastName , MobileNo ,Email,Country , MFCN ,PhoneCode,StatusOfFax";
        public int Id { get; set; }

        [Required(ErrorMessage ="Enter First Name")]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }

        public string LastName { get; set; }
        [Required(ErrorMessage = "Enter Last Name")]
        public string MobileNo { get; set; }
        [Required(ErrorMessage = "Enter Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Please Enter the MFCN")]
        public string MFCN { get; set; }
        public string PhoneCode { get; set; }
        public FaxingStatus StatusOfFax { get; set; }
    }
}