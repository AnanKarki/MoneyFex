using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.ViewModels
{

    public class MyMoneyFaxCardPurchaseSheetAndTopupViewModel {


        public Options Options { get; set; }
        public List<MyMoneyFaxCardPurchaseSheetViewModel> purchaseSheetViewModels { get; set; }

        public List<MyMoneyFaxCardTopUpSheetViewModel> topUpSheetViewModels { get; set; }
    }
    public class MyMoneyFaxCardPurchaseSheetViewModel
    {
        public int TransactionId { get; set; }
        public string BusinessMerhantName { get; set; }
        public string BusinessMobileNo { get; set; }
        public string PaymentReference { get; set; }

        public string AmoountSpent { get; set; }

        public string Date { get; set; }
        public string Time { get; set; }

        public DateTime TransactionDate { get; set; }
        
    }
    public class MyMoneyFaxCardTopUpSheetViewModel
    {
        public int TransactionId { get; set; }
        public string CarduserName { get; set; }
        public string MFTCCardNumber { get; set; }
        public string PaymentReference { get; set; }

        public string AmoountSpent { get; set; }

        public string Date { get; set; }
        public string Time { get; set; }

        public DateTime TransactionDate { get; set; }

    }
    public enum Options
    {
        [Display(Name = "Select Card Usage History Option")]
        Select_Card_Usage_History_Option,
        [Display(Name = "Purchases")]
        Card_Purchase,
        [Display(Name = "Payments")]
        TopUp,

    }
}