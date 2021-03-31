using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AgentPayInvoiceViewModel
    {
        public int Id{ get; set; }
        public string Name{ get; set; }
        public string InvoiceNo{ get; set; }
        public string CountryCurrencySymbol{ get; set; }
        public decimal  Fee{ get; set; }
        public decimal  YouPay{ get; set; }
        public decimal  TheyReceive{ get; set; }
    }
}