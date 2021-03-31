using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class FaxingTransactionViewModel
    {
       
        public string TransactionID { get; set; }
        public string UserID { get; set; }
        public string ReceiversID { get; set; }
        public string FaxingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public string FaxingAmount { get; set; }
        public bool IsSendSuccess { get; set;}
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public bool IsDeleted { get; set; }

    }
}