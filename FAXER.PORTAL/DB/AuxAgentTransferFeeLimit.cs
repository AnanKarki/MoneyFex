using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class AuxAgentTransferFeeLimit
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public string SendingCurrency { get; set; }
        public string SendingCountry { get; set; }
        public string ReceivingCurrency { get; set; }
        public string ReceivingCountry { get; set; }
        public FeeType FeeType { get; set; }
        public TranfserFeeRange Range { get; set; }
        public string OtherRange { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public decimal TransferFee { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}