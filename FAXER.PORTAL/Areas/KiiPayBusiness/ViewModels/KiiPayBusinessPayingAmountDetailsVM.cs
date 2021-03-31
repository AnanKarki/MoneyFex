using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessInternationalPaymentPayingAmountDetailsVM
    {
        public const string BindProperty = " SendingAmount,ReceivingAmount ,TotalAmount , Fee,ExchangeRate ,PaymentReference , SendingCountryCurrency,  " +
            "SendingCountryCurrencySymbol,ReceivingCountryCurrency ,ReceivingCountryCurrencySymbol , ReceiverName";
        #region paying Amount details 

        [Range(1, int.MaxValue, ErrorMessage = "Please enter a amount greater than {0}")]
        public decimal SendingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }

        public decimal TotalAmount { get; set; }
        
        public decimal Fee { get; set; }

        public decimal ExchangeRate { get; set; }

        [Required(ErrorMessage = "Please enter the payment reference")]
        public string PaymentReference { get; set; }

        #endregion
        #region Sending And Receiving Country Currency Details 

        public string SendingCountryCurrency { get; set; }
        public string SendingCountryCurrencySymbol { get; set; }
        public string ReceivingCountryCurrency { get; set; }
        public string ReceivingCountryCurrencySymbol { get; set; }

        public string ReceiverName { get; set; }

        #endregion

    }

    public class KiiPayBusinessLocalPaymentPayingAmountDetailsVM
    {
        public const string BindProperty = " ReceiverName,SendingAmount ,SendingCountryCurrency , SendingCountryCurrencySymbol ,PaymentReference , SendSms";

        public string ReceiverName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Enter amount")]
        public decimal SendingAmount { get; set; }

        public string SendingCountryCurrency { get; set; }
        public string SendingCountryCurrencySymbol { get; set; }
        [Required(ErrorMessage = "Enter  payment reference")]
        public string PaymentReference { get; set; }

        public bool SendSms { get; set; }
    }
}