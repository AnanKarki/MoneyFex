using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class TransferExchangeRateHistory
    {
        public int Id { get; set; }
        public string SendingCurrency { get; set; }
        public string SendingCountry { get; set; }
        public string ReceivingCurrency { get; set; }
        public string ReceivingCountry { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public TransactionTransferType TransferType { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public int? AgentId { get; set; }
        public decimal Rate { get; set; }
        public decimal FromRange { get; set; }
        public decimal ToRange { get; set; }

    }
}