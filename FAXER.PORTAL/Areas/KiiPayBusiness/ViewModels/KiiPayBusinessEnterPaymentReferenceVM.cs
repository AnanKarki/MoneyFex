using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessEnterPaymentReferenceVM
    {
        public const string BindProperty = " ReferenceNo0, ReferenceNo1,ReferenceNo2";
        [Required(ErrorMessage = "Please enter 3 Digit Reference No.")]
        public string ReferenceNo0 { get; set; }
        [Required(ErrorMessage = "Please enter 3 Digit Reference No.")]
        public string ReferenceNo1 { get; set; }
        [Required(ErrorMessage = "Please enter 3 Digit Reference No.")]
        public string ReferenceNo2 { get; set; }

    }
}