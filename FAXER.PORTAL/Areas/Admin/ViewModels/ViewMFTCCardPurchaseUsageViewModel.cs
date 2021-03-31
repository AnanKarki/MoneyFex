using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewMFTCCardPurchaseUsageViewModel
    {
        public int Id { get; set; }
        public string FaxerFullName { get; set; }
        public string CardUserFullName { get; set; }
        
        public decimal PurchaseAmount { get; set; }
        public string Currency { get; set; }
        public string PurchaseTime { get; set; }
        public string PurchaseDate { get; set; }
        public string BusinessMerchantVerifier { get; set; }
        public string BusinessMerchantName { get; set; }
        public string BusinessMerchantBMFSCode { get; set; }
        public string PaymentRejection { get; set; }
    }
}