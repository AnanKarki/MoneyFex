using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class AuxAgentExchangeRateLimit
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public string SendingCurrency { get; set; }
        public string SendingCountry { get; set; }
        public string ReceivingCurrency { get; set; }
        public string ReceivingCountry { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}