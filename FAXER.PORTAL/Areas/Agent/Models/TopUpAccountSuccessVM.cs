using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class TopUpAccountSuccessVM
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string ReceiverAccountNo { get; set; }
    }
}