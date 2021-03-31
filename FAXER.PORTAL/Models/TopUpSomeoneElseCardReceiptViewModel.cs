using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class TopUpSomeoneElseCardReceiptViewModel
    {

        public string ReceiptNO { get; set; }

        public string FaxerFullName { get; set; }
        public DateTime TransactionDate { get; set; }

        public string FaxingAmount { get; set; }

        public string ExchangeRate { get; set; }

        public string FaxingFee { get; set; }

        public string ReceivingAmount { get; set; }

        public string TopUpReference { get; set; }
    }
}