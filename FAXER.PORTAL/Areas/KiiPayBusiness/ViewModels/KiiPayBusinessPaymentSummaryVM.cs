using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessPaymentSummaryVM
    {
        public const string BindProperty = "ReceiverWalletId, ReceiverName ,TotalAmount ,Fee ,SendingAmount ,ReceivingAmount , SmsFee,ExchangeRate , PaymentReference" +
            ",SendingCountryCurrency ,SendingCountryCurrencySymbol ,ReceivingCountryCurrency ,ReceivingCountryCurrencySymbol";

        public int ReceiverWalletId { get; set; }

        public string ReceiverName { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal Fee { get; set; }
        public decimal SendingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public decimal SmsFee { get; set; }

        public decimal ExchangeRate { get; set; }

        public string PaymentReference { get; set; }

        public string SendingCountryCurrency { get; set; }
        public string SendingCountryCurrencySymbol { get; set; }


        public string ReceivingCountryCurrency { get; set; }
        public string ReceivingCountryCurrencySymbol { get; set; }




    }
}