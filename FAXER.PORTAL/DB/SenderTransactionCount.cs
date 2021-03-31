using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class SenderTransactionCount
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public int TransactionCount { get; set; }
        public CrediDebitCardUsageFrequency Frequency { get; set; }
    }
}