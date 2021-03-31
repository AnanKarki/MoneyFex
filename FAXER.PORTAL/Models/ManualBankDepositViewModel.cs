using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class ManualBankDepositViewModel
    {
        public int Id { get; set; }
        public string SendingCountry { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public decimal TotalAmount { get; set; }
        public string ReferenceNo { get; set; }
        public string TransactionDate { get; set; }
        public BankDepositStatus Status { get; set; }
        public string StatusName { get; set; }
    }
}