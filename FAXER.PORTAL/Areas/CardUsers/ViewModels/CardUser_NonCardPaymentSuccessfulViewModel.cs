using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.ViewModels
{
    public class CardUser_NonCardPaymentSuccessfulViewModel
    {

        public string MFCNNumber { get; set; }

        public string ReceiverName { get; set; }
        public string ReceiverCountry { get; set; }
        public string SentAmount { get; set; }
        public string ReceiveAmount { get; set; }
        public string ExchangeRate { get; set; }
        public string ReceiveOption { get; set; }

    }
}