using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    /// <summary>
    /// the transaction will go 
    /// through manual approval if the 
    /// </summary>
    public class ManualApprovalTransactionCountry
    {
        public int Id { get; set; }
        public string ReceivingCountry { get; set; }
        public string ReceivingCurrency { get; set; }
        public TransactionTransferMethod TransactionTransferMethod { get; set; }

        public bool IsManualPaymentEnabled { get; set; }

    }
}