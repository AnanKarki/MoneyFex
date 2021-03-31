using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class MyBusinessMoneyFaxSalesSheetViewModel
    {
        public int Id { get; set; }

        public string NameOfBuyer { get; set; }

        public string Amount { get; set; }
        public string PaymentReference { get; set; }
        public string Country { get; set; }
        public string City { get; set; }

        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string Currency { get; set; }
        public string CurrencySymbol { get; set; }
    }
}