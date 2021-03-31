using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class InvoicingViewModel
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Date { get; set; }
        public string InvoiceNo { get; set; }
        public string CurrencySymbol { get; set; }
        public string StatusName { get; set; }
        public decimal Amount { get; set; }


        #region For AgentInvoicing
        public string Name { get; set; }
        public string AccountNo { get; set; }
        public string AgentId { get; set; }
        #endregion

        #region For PartnerInvoicing

        public int PartnerId {get; set; }
        
        #endregion

    }

}