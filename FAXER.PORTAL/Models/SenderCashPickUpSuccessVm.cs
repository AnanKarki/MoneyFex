using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderCashPickUpSuccessVm
    {
        public int Id { get; set; }
        public string SendingCurrency { get; set; }
        public decimal SentAmount{ get; set; }
        public string ReceiverName{ get; set; }
        public string MFCNNumber{ get; set; }
    }
}