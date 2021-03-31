using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class BusinessMoneyTransferInProgressViewModel
    {
        public int TransactionId { get; set; }
        public int ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverCountry { get; set; }

        public string ReceiverCity { get; set; }
        public string TransferAmount { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }


        public string MFCN { get; set; }
        public string StatusOfTransfer { get; set; }
    }
}