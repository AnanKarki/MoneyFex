using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels.SignUpViewModel
{
    public class kiiPayBusinessInfoVM
    {
        public const string BindProperty = " CountryOfIncorporation ,BusinessName  ,RegistrationNumber , BusinessType ";
        [Required(ErrorMessage ="Please select your country of incorporation")]
        public string CountryOfIncorporation { get; set; }

        [Required(ErrorMessage = "Please select your country of incorporation")]
        public string BusinessName { get; set; }


        [Required(ErrorMessage = "Please select your country of incorporation")]
        public string RegistrationNumber { get; set; }


        [Range( 1 , 7 , ErrorMessage ="Please select the business type")]
        public BusinessType BusinessType { get; set; }



    }
}