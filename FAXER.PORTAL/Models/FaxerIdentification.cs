using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using FAXER.PORTAL.Areas.Admin.ViewModels;

namespace FAXER.PORTAL.Models
{
    public class FaxerIdentification
    {

        public const string BindProperty = "IdCardType,IdCardNumber,IdCardExpiringDate,IssuingCountry,CardUrl,CheckAmount";

        [Required(ErrorMessage ="Enter Id Card Type")]
        [Display(Name = "ID Card Type")]
        public string IdCardType { get; set; }


        [Required(ErrorMessage ="Enter Id Card Number")]
        [Display(Name = "ID Card Number")]
        public string IdCardNumber { get; set; }


        [Required(ErrorMessage ="Enter Id Card Expiring Date")]
        [Display(Name = "ID Card Expiring Date")]
        public DateTime IdCardExpiringDate { get; set; }

        [Required(ErrorMessage ="Select Issuing Country")]
        [Display(Name = "Issuing Country")]
        public string  IssuingCountry { get; set; }
       
        public string CardUrl { get; set; }

        public bool CheckAmount { get; set; }

        public static implicit operator FaxerIdentification(ViewRegisteredFaxersViewModel v)
        {
            throw new NotImplementedException();
        }
    }
}