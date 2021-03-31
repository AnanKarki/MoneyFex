using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderAddMoneyToWalletSuccessViewModel
    {
        public string CurrencyCode { get; set; }
        public decimal SendingBalance { get; set; }

        public string SendToName { get; set; }
    }
}