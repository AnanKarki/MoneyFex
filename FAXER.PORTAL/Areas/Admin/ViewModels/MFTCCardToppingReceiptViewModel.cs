using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class MFTCCardToppingReceiptViewModel
    {
        public int Id { get; set; }
        public string ReceiptNumber { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string FaxerFullName { get; set; }

        public string FaxerCountry { get; set; }

        public string MFTCCardNumber { get; set; }
        public string CardUserFullName { get; set; }

        public string CardUserCountry { get; set; }
        public string CardUserCity { get; set; }

        public string AmountTopUp { get; set; }
        public string ExchangeRate { get; set; }
        public string AmountInLocalCurrency { get; set; }
        public string Fee { get; set; }
        public string BalanceOnCard { get; set; }
        public string StaffName { get; set; }
        public string StaffCode { get; set; }

        public string StaffLoginCode { get; set; }

        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }
    }
}