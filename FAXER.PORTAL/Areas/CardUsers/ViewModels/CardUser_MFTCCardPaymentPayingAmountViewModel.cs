using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.ViewModels
{
    public class CardUser_MFTCCardPaymentPayingAmountViewModel
    {
        public const string BindProperty = " TopUpAmount , ReceivingAmount, IncludingFee, PaymentReference";
        public decimal TopUpAmount { get; set; }

        public decimal ReceivingAmount { get; set; }
        public bool IncludingFee { get; set; }

        public string PaymentReference { get; set; }
    }
}