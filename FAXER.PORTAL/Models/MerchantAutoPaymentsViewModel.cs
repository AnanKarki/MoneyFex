using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class MerchantAutoPaymentsViewModel
    {
        public int Id { get; set; }
        public string MerchantName { get; set; }
        public string AccountNo { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string AutoAmount { get; set; }
        public string PaymentFrequency { get; set; }

        public DB.AutoPaymentFrequency Frequency { get; set; }
        public string FrequencyDetails { get; set; }

        public string PaymentReference { get; set; }
        public string EnableAutoPayment { get; set; }
    }

    public class MerchantInfoDropDowmViewModel {

        public int Id { get; set; }

        public string MerchantName { get; set; }
    }
    public enum Month
    {
    [Display(Name ="Select Month")]
        Month = 0,
        January = 01,
        February = 02,
        March = 03,
        April = 04,
        May = 05,
        June = 06,
        July = 07,
        August = 08,
        September = 09,
        October = 10,
        November = 11,
        December = 12
    }
    public enum Months
    {
  
        Month = 0,
        January = 01,
        February = 02,
        March = 03,
        April = 04,
        May = 05,
        June = 06,
        July = 07,
        August = 08,
        September = 09,
        October = 10,
        November = 11,
        December = 12
    }
}