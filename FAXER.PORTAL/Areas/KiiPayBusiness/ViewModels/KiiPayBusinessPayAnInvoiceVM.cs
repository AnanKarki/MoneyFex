using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessPayAnInvoiceVM
    {
    }
    public class KiiPayBusinessPayAnInvoiceSummaryvm
    {
        public int Id { get; set; }
        public string BusinessName { get; set; }
        public string InvoiceNo { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public decimal TotalAmount { get; set; }
        public string CurrencySymbol { get; set; }
    }
}