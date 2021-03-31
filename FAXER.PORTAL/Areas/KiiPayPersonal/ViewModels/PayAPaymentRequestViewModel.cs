using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class PayAPaymentRequestViewModel
    {
        public const string BindProperty = "Id ,PhotoUrl ,ReceiverName ,ReceiverWalletId , Amount , Fee ,PayingAmount  ," +
              " ReceivingAmount, ExchangeRate, ReceiverCurrencySymbol";

        public int Id { get; set; }
        public string PhotoUrl { get; set; }
        public string ReceiverName { get; set; }
        public int ReceiverWalletId { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public decimal PayingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public string ReceiverCurrencySymbol { get; set; }
        
    }
}