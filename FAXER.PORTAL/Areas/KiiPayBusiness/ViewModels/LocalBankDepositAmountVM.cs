using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class LocalBankDepositAmountVM
    {
        public const string BindProperty = "PayingToBankName , Amount, PaymentReference , SendSMS";
        public string PayingToBankName { get; set; }


        [Range(1 , int.MaxValue ,ErrorMessage = "Please enter the amount greater than {0}..")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage ="Please enter the payment reference..")]
        public string PaymentReference { get; set; }    
        public bool SendSMS { get; set; }


    }
}