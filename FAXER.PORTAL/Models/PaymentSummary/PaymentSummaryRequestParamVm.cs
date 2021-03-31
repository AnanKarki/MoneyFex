using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models.PaymentSummary
{
    public class PaymentSummaryRequestParamVm
    {

        public decimal SendingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }
        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public bool IsReceivingAmount { get; set; }
        public bool IsAuxAgnet { get; set; }
        public bool ForAgent { get; set; }
        public int AgentId { get; set; }
        public int TransferMethod { get; set; }
        public int TransferType { get; set; }
        public bool IsIncludeFaxingFee { get; set; }

        public int SenderId { get; set; }
    }


}