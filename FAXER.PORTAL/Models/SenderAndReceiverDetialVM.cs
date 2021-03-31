using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderAndReceiverDetialVM
    {

        public int WalletOperatorId { get; set; }

        public int SenderId { get; set; }

        public string SenderCountry { get; set; }

        /// <summary>
        /// Receiver Id Can be Kiipay Wallet Id OR Business Wallet Id 
        /// </summary>

        public int ReceiverId { get; set; }

        public string ReceiverCountry { get; set; }

        public string ReceiverMobileNo { get; set; }
        public int SenderWalletId { get; set; }
        public string SenderPhoneNo { get; set; }

        public int TransactionSummaryId { get; set; }

        public string ReceivingCurrency { get; set; }
        public string SendingCurrency { get; set; }

    }
}