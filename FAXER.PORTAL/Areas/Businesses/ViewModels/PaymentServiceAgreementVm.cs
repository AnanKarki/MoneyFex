using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class PaymentServiceAgreementVm
    {

        public const string BindProperty = "IhaveRead , IAccept ";


        [Required, DisplayName("Agreement")]
        public bool IhaveRead { get; set; }
        [Required, DisplayName("Agreement")]
        public bool IAccept { get; set; } 
    }
}