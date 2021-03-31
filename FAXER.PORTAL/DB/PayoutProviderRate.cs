using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class PayoutProviderRate
    {
        public int Id { get; set; }
        public string SendingCurrency { get; set; }
        public string RecevingCurrency { get; set; }
        public string SendingCountry { get; set; }
        public string RecevingCountry { get; set; }
        public Apiservice PayoutProvider { get; set; }
        public decimal Rate { get; set; }
    }
}