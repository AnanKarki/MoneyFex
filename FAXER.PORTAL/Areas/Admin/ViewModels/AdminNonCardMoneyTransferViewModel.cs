using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AdminNonCardMoneyTransferViewModel
    {
        public int Id { get; set; }
        public string MFReceiptNumber { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionTime { get; set; }
        public string FaxerFullName { get; set; }
        public string MFCN { get; set; }
        public string ReceiverFullName { get; set; }
        public string Telephone { get; set; }
        public string StaffName { get; set; }
        public string StaffCode { get; set; }
        public string AmountSent { get; set; }
        public string ExchangeRate { get; set; }
        public string Fee { get; set; }
        public string AmountReceived { get; set; }
        public string TotalAmountSentAndFee { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }

        public string SenderTelephoneNo { get; set; }
        public string StaffLoginCode { get; set; }




    }
}