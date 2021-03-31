using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.ViewModels
{
    public class MFCTCardSignUpViewModel
    {
        public const string BindProperty = " Surname, DateOfBirth ,MFTCCardNumber ,EmailAddress , Password , ConfirmPassword ,IdCardType  , IdCardNumber , IdCardExpiringDate , IssuingCountry" +
            ",Invalidcard ,Confirm";
        [Required]
        public string Surname { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string MFTCCardNumber { get; set; }
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Password didn't match"),]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "ID Card Type")]
        public string IdCardType { get; set; }


        [Required]
        [Display(Name = "ID Card Number")]
        public string IdCardNumber { get; set; }


        [Required]
        [Display(Name = "ID Card Expiring Date")]
        public DateTime IdCardExpiringDate { get; set; }

        [Required]
        [Display(Name = "Issuing Country")]
        public string IssuingCountry { get; set; }
        public string Invalidcard { get; set; }

        public bool Confirm { get; set; }

    }
}