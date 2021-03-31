using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class IntroductoryRateByCurrency
    {
        public int Id { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }
        public TransactionTransferType TransactionTransferType { get; set; }
        public TransactionTransferMethod TransactionTransferMethod { get; set; }
        public int AgentId { get; set; }
        public string Range { get; set; }
        public decimal FromRange { get; set; }
        public decimal ToRange { get; set; }
        public decimal Rate { get; set; }
        public int NoOfTransaction { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
    }
}