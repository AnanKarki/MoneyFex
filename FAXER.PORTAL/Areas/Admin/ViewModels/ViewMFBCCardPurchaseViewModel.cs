using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewMFBCCardPurchaseViewModel
    {
        public int Id { get; set; }
        public string BusinessName { get; set; }
        public string CardUserFullName { get; set; }
        public decimal PurchaseAmt { get; set; }
        public string Currency { get; set; }
        public string PurchaseTime { get; set; }
        public string PurchaseDate { get; set; }
        public string MerchantVerifier { get; set; }
        public string MerchantName { get; set; }
        public string MerchantBMFSCode { get; set; }

        public DateTime TransactionDate { get; set; }

        public string PaymentType { get; set; }



    }
}