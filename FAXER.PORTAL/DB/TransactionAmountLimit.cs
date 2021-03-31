using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class TransactionAmountLimit
    {
        public int Id { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }
        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public int SenderId { get; set; }
        public int StaffId { get; set; }
        public decimal Amount { get; set; }
        public Module ForModule { get; set; }
    }
}