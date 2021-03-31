using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderKiiPayWalletUserRegistrationViewModel
    {
        public const string BindProperty = "Id,Country,FullName,MobileNo,Email,CountryPhoneCode";

        [Key]
        public int Id { get; set; }
        public string Country { get; set; }
        [Required(ErrorMessage = "Enter Full Name")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Enter Mobile Number")]
        public string MobileNo { get; set; }
        //[Required(ErrorMessage = "This field is required")]
        public string Email { get; set; }

        public string CountryPhoneCode { get; set; }

    }
}