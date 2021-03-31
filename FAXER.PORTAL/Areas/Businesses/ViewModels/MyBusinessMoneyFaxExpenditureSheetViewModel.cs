using FAXER.PORTAL.Areas.CardUsers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{

    public class MyBusinessMoneyFaxExpenditureSheetViewModel {

        public List<MyBusinessMoneyFaxPurchaseViewModel> purchaseViewModels { get; set; }

        public List<MyBusinessMoneyFaxTopUpViewModel> topUpViewModels { get; set; }

        public Options Options { get; set; }
    }
    public class MyBusinessMoneyFaxPurchaseViewModel
    {

        public int Id { get; set; }

        public string NameOfMerchant { get; set; }

        public string MerchantAccountNo { get; set; }
        public string PaymentReference { get; set; }
        public string AmountSpent { get; set; }

        public string Country { get; set; }
        public string City { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }
        public DateTime TransactionDateTime { get; set; }

    }
    public class MyBusinessMoneyFaxTopUpViewModel {
        public int Id { get; set; }

        public string CardUserName { get; set; }

        public string MFTCCardNumber { get; set; }
        public string PaymentReference { get; set; }
        public string AmountSpent { get; set; }

        public string Country { get; set; }
        public string City { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }
        public DateTime TransactionDateTime { get; set; }

    }
}