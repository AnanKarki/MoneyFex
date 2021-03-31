using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class MFTCCardTopUpByMerchantSuccessfulViewModel
    {

        public string MFTCCardNumber { get; set; }

        public string CardUserName { get; set; }

        public string CardUserCountry { get; set; }

        public decimal FaxingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }

        public decimal ExchangeRate { get; set; }

        public string ReceiveOption { get; set; }
    }
}