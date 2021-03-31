using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.ViewModels
{
    public class CardUser_NonCardPaymentDetailsViewModel
    {
        public decimal faxingAmount { get; set; }
        public decimal FaxingFee { get; set; }
        public decimal TotalAmountIncludingFee { get; set; }
        public decimal CurrentExchangeRate { get; set; }
        public decimal AmountToBeReceived { get; set; }
        public string PaymentReference { get; set; }
    }
}