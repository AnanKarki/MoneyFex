using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderAddMoneySuccessVM
    {
        public int Id { get; set; }
        public string Currnecy { get; set; }
        public string ReceiverName { get; set; }
        public decimal Amount { get; set; }
    }
}