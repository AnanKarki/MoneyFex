using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class DailyRatesViewModel
    {
        public int Id { get; set; }
        public string SendingCountry { get; set; }
        public string SendingCountryLower { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCountry { get; set; }
        public string ReceivingCountryLower { get; set; }
        public string ReceivingCurrency { get; set; }
        public decimal ReceivingAmount{ get; set; }
        public DateTime? Date { get; set; }
        public string ShrotDate { get; set; }
    }
}