using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransferZero.Sdk.Model;

namespace FAXER.PORTAL.Models
{
    public class TransactionPendingViewModel
    {
        public int TransactionId { get; set; }
        public int SenderId { get; set; }
        public TransactionServiceType TransferMethod { get; set; }
        public  bool IsTransactionPending { get; set; }
        public  string TransactionNumber { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal SendingAmount { get; set; }
        public decimal Fee { get; set; }
        public string ReceiverFullName { get; set; }
        public string SendingCurrency { get; set; }
        public string Receivingurrency { get; set; }
        public string ReceivingCountry { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public string BankCode { get; set; }
        public string MFCN { get; set; }
        public string WalletName { get; set; }
        public string ReceiptNumber { get; set; }

        public string MobileNo { get; set; }


    }
}