using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.Common
{
    public class MobileCurrencyDropDownViewModel
    {
        public int Id { get; set; }
        public string CurrencyName { get; set; }
        public string SendingCountryCode { get; set; }
        public string ReceivingCountryCode { get; set; }

        public string SendingCurrency { get; set; }

        public string ReceivingCurrency { get; set; }

        public string SendingCurrencySymbol { get; set; }


        public string ReceivingCurrencySymbol { get; set; }


        public decimal ExchangeRate { get; set; }




    }
}