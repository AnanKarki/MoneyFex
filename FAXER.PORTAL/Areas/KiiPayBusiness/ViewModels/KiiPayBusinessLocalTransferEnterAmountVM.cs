using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessLocalTransferEnterAmountVM
    {
        public const string BindProperty = "BusinessName, Amount,CurrencySymbol ,CurrencyCode ,PaymentReference ,SendSMS  ";
        public string BusinessName { get; set; }
        [Range(1 , int.MaxValue , ErrorMessage ="Please enter the amount greater than {0}")]
        public decimal Amount { get; set; }
        public string CurrencySymbol { get; set; }

        public string CurrencyCode { get; set; }
        [Required(ErrorMessage ="Please enter the payment reference")]
        public string PaymentReference { get; set; }
        public bool SendSMS { get; set; }


    }

    public class KiiPayBusinessInternationalTransferEnterAmountVM
    {
        public const string BindProperty = "BusinessName, SendingAmount,RecevingAmount ,SendingCurrencySymbol ,RecevingCurrencySymbol ,SendingCurrencyCode ,RecevingCurrencyCode" +
            ", Fee,TotalAmount ,ExchangeRate ,PaymentReference ";

        public string BusinessName { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter the sendingamount greater than {0}")]
        public decimal SendingAmount { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter the recevingamount greater than {0}")]
        public decimal RecevingAmount { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string RecevingCurrencySymbol { get; set; }

        public string SendingCurrencyCode { get; set; }
        public string RecevingCurrencyCode { get; set; }

        public decimal Fee { get; set; }

        public decimal TotalAmount  { get; set; }
        public decimal ExchangeRate { get; set; }
        [Required(ErrorMessage = "Please enter the payment reference")]
        public string PaymentReference { get; set; }

    }
}