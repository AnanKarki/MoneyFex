using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class TransactionError
    {
        public int Id { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public int TransactionId { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime Date { get; set; }
        public TransactionErrorStatus TransactionErrorStatus { get; set; }
    }
    public enum TransactionErrorStatus
    {
        ErrorState,
        ReleasedState
    }
}