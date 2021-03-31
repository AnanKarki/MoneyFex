using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SetLimitFaxTopUpCardViewModel
    {
        public const string BindProperty = "MFTCCardId,MFTCCardNumber,CurrentBalance,PreviousCashWithDrawLimit," +
            "WithDrawAmount,WithDrawAutoPaymentFrequency,PreviousPurchasingLimit,PurchasingAmount,PurchasingAutoPaymentFrequency,CardUserCurrency" +
            ",UpdatePurchaseLimit,UpdateWithdrawlLimit,UpdateBothLimitType";

        public int MFTCCardId { get; set; }
        public string MFTCCardNumber { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal PreviousCashWithDrawLimit { get; set; }
        public decimal WithDrawAmount { get; set; }
        public CardLimitType WithDrawAutoPaymentFrequency { get; set; }
        public decimal PreviousPurchasingLimit { get; set; }
        public decimal PurchasingAmount { get; set; }
        public AutoPaymentFrequency PurchasingAutoPaymentFrequency { get; set; }
        public string CardUserCurrency { get; set; }

        public bool UpdatePurchaseLimit { get; set; }

        public bool UpdateWithdrawlLimit { get; set; }

        public bool UpdateBothLimitType { get; set; }

    }
}