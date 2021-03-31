using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AdminPayGoodsAndServicesReceiptViewModel
    {
        public int Id { get; set; }
        public string ReceiptNumber { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string FaxerFullName { get; set; }

        public string FaxerCountry { get; set; }

        public string BusinessMerchantName { get; set; }
        public string BusinessMFCode { get; set; }
        public string BusinessCountry { get; set; }
        public string BusinessCity { get; set; }

        public string AmountPaid { get; set; }
        public string ExchangeRate { get; set; }
        public string AmountInLocalCurrency { get; set; }
        public string Fee { get; set; }
        public string StaffName { get; set; }
        public string StaffCode { get; set; }

        public string StaffLoginCode { get; set; }

        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }
    }
}