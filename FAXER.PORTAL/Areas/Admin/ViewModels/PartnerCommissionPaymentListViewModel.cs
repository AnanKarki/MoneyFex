using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class PartnerCommissionPaymentListViewModel
    {

        public int Id{ get; set; }
        public string Status{ get; set; }
        public string Type{ get; set; }

        public string CurrencySymbol{ get; set; }
   
        public decimal Amount{ get; set; }
        public string DueDate{ get; set; }
        #region ForPartnerAgentTransactionList
        public string Method { get; set; }
        public decimal Fee { get; set; }
        public string Indentifier{ get; set; }
        public string DateandTime{ get; set; }
        public string StaffName { get; set; }

        #endregion
    }
}