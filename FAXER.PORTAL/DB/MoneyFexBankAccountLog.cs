using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class MoneyFexBankAccountLog
    {
        public int Id { get; set; }
        public int TranscationId { get; set; }
        public TransactionTransferMethod TrasnferMethod { get; set; }
        public string PaymentReference { get; set; }
        public bool IsConfirmed { get; set; }
    }
}