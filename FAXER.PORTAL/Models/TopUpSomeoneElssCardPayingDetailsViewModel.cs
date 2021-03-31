using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class TopUpSomeoneElseCardPayingDetailsViewModel
    {

        public decimal faxingAmount { get; set; }
        public decimal FaxingFee { get; set; }
        public decimal TotalAmountIncludingFee { get; set; }
        public decimal CurrentExchangeRate { get; set; }
        public decimal AmountToBeReceivedByCardUser { get; set; }
        public string PaymentReference { get; set; }
        public string FaxingCurrency { get; set; }
        public string FaxingCurrencySymbol { get; set; }
        public string ReceivingCurrency { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
    }
}