using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class IntroductoryFeeByCurrency
    {
        public int Id { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }
        public TransactionTransferType TransferType { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public decimal FromRange { get; set; }
        public decimal ToRange { get; set; }
        public string OtherRange { get; set; }
        public FeeType FeeType { get; set; }
        public decimal Fee { get; set; }
        public DateTime CreatedDate { get; set; }
        public NumberOfTransaction NumberOfTransaction { get; set; }
        public int AgentId { get; set; }
    }
}