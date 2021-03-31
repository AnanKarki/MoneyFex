using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.ViewModels
{
    public class MFTCCardPaymentByCardUserSuccessfulViewModel
    {
        public string ReceiverName { get; set; }
        public string ReceiverCardNumber { get; set; }
        public string ReceiverCountry { get; set; }
        public string TopUpAmount { get; set; }
        public string ExchangeRate { get; set; }
        public string ReceiveAmount { get; set; }
        public string Receiveoption { get; set; }

    }
}