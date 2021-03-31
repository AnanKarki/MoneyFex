using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class PayoutFlowControl
    {
        public int Id { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public Apiservice PayoutApi { get; set; }
        public bool IsPayoutEnabled { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    } 
    public class PayoutFlowControlDetails
    {
        public int Id { get; set; }
        public int PayoutProviderId{ get; set; }
        public int PayoutFlowControlId{ get; set; }

        public PayoutFlowControl PayoutFlowControl { get; set; }
    }
}